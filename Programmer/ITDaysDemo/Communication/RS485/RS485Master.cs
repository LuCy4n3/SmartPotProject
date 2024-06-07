using System.Collections;
using System.Diagnostics;
using ITDaysDemo.Utils;
using RJCP.IO.Ports;

namespace ITDaysDemo.Communication.RS485
{
    internal class RS485Master
    {
        private readonly SerialPortStream _RS485SerialPortStream;
        private readonly SerialPortStream _temperatureAndLightsRadioStream;
        private readonly SerialPortStream _coSensorRadioStream; 
        public static RS485Master Instance { get; }
        public bool LightStatus { get; set; }
        public int DimmingValue { get; set; } = 100;
        static RS485Master()
        {
            Instance = new RS485Master();
        }
        private RS485Master()
        {
            _RS485SerialPortStream = new SerialPortStream("COM3", 9600);
            _RS485SerialPortStream.Open();
            _RS485SerialPortStream.RtsEnable = _RS485SerialPortStream.DtrEnable = true; //set MAX485 in receive mode (pin will be LOW)


            _temperatureAndLightsRadioStream = new SerialPortStream("COM11", 2400);
            _temperatureAndLightsRadioStream.Open();
            _temperatureAndLightsRadioStream.RtsEnable = _temperatureAndLightsRadioStream.DtrEnable = false; //set radio pin settings to HIGH

            _coSensorRadioStream = new("COM10", 9600);
            _coSensorRadioStream.Open();
            _coSensorRadioStream.RtsEnable = _coSensorRadioStream.DtrEnable = false; //set radio pin settings to HIGH
        }
        public static bool BackroundReadsEnabled { get; set; }
        private static byte[] incommingData = new byte[8];
        public async Task ListenForCommandsAsync()
        {
            Stopwatch lastTimeDataReceived = Stopwatch.StartNew();
            LightStatus = false;
            DimmingValue = 100;
            while (true)
            {
                await Task.Delay(1);
                if (BackroundReadsEnabled)
                {
                    while (_RS485SerialPortStream.BytesToRead > 0)
                    {
                        if (lastTimeDataReceived.Elapsed.TotalMilliseconds > 50000)
                        {
                            incommingData = new byte[8];
                        }
                        lastTimeDataReceived.Restart();
                        for (var i = 0; i < incommingData.Length - 1; i++)
                        {
                            incommingData[i] = incommingData[i + 1];
                        }

                        incommingData[incommingData.Length - 1] = (byte)_RS485SerialPortStream.ReadByte();
                        HandleCommand(incommingData);
                    }
                }
            }
        }

        private void HandleCommand(byte[] bytes)
        {
            var hexStr = ToHexString(bytes).Replace("-", "");
            bool hasValidCommand = false;
            if (hexStr.Equals("0x020300020900E269", StringComparison.OrdinalIgnoreCase))
            {
                LightOnOffPressed();
            }
            if (hexStr.Equals("0x020300020A00E299", StringComparison.OrdinalIgnoreCase))
            {
                DimmingPressed();
            }

            if (bytes[0] == 0x77) //current and voltage measure
            {
                if (ModRTU_CRC(bytes, 6) == BitConverter.ToUInt16(bytes, 6))
                {
                    var current = BitConverter.ToUInt16(bytes, 2);
                    var voltage = BitConverter.ToUInt16(bytes, 4);
                    var mainForm = Application.OpenForms.OfType<Demo>().Single();
                    mainForm.Invoke(() =>
                    {
                        mainForm.lblCurrent.Text = $"Current:{current}";
                        mainForm.lblVoltage.Text = $"Voltage:{voltage}";
                        mainForm.lblVoltageAndCurrentReceivedTime.Text = $"Received Time:{DateTime.Now}";
                    });
                }
            }
        }

        public void LightOnOffPressed()
        {
            LightStatus = !LightStatus;
            SetLightDimmingLevel();
            SetLightsStatusLabels();
        }

        public void DimmingPressed()
        {
            DimmingValue -= 5;
            if (DimmingValue <= 0)
                DimmingValue = 100;
            if (LightStatus)
            {
                SetLightDimmingLevel();
            }
            SetLightsStatusLabels();
        }

        private void SetLightsStatusLabels()
        {
            var form = Application.OpenForms.OfType<Demo>().Single();
            form.Invoke(() =>
            {
                form.lblDimmingLevel.Text = $"Dimming Level:{DimmingValue}";
                form.lblLightStatus.Text = $"Light:{(LightStatus ? "ON" : "OFF")}";
            });
        }
        public async Task ReadVoltageAndCurrentAsync()
        {
            while (true)
            {
                await Task.Delay(300);
                var commandSequence = GetCommandSequence(ProtocolSettings.VoltageAndCurrentMeasureId, ProtocolSettings.CMDReadVoltageAndCurrent, 0);
                _RS485SerialPortStream.ReadExisting();//clean receive buffer
                _RS485SerialPortStream.RtsEnable = _RS485SerialPortStream.DtrEnable = false; //set MAX485 transceiver in send mode
                await _RS485SerialPortStream.WriteAsync(commandSequence.ToArray());
                _RS485SerialPortStream.WaitForDataToBeSentOnWire(commandSequence.Count);
                _RS485SerialPortStream.RtsEnable = _RS485SerialPortStream.DtrEnable = true; //set MAX485 transceiver in read mode
                byte[] response = await ReadBytesAsync();
                if (response != null)
                {
                    //current = 
                    //voltage = 
                }
            }
        }

        private async Task<byte[]> ReadBytesAsync()
        {
            const int timeoutMs = 500;
            Stopwatch sw = Stopwatch.StartNew();
            List<byte> buffer = new List<byte>();
            while (sw.Elapsed.TotalMilliseconds < timeoutMs)
            {
                if (_RS485SerialPortStream.BytesToRead > 0)
                {
                    buffer.Add((byte)_RS485SerialPortStream.ReadByte());
                    if (DataPackageIsValid(buffer.ToArray(), out var dataBytes))
                        return dataBytes;
                }
            }
            return null;
        }

        private bool DataPackageIsValid(byte[] buffer, out byte[] validPackage)
        {
            validPackage = null;
            if (buffer.Length < 3)
                return false;

            UInt16 crcCandidate = BitConverter.ToUInt16(buffer, buffer.Length - 2);
            for (var i = 0; i < buffer.Length - 2; i++)
            {
                if (ModRTU_CRC(buffer.Skip(i).ToArray(), buffer.Length - 2 - i) == crcCandidate)
                {
                    validPackage = buffer.Skip(i).ToArray();
                    return true;
                }
            }
            return false;
        }

        private List<Byte> GetCommandSequence(byte deviceId, UInt16 cmd, UInt16 commandValue)
        {
            List<Byte> sequence = new[] { deviceId }
                .Concat(new[] { ProtocolSettings.RTUFunctionWrite })
                .Concat(BitConverter.GetBytes(cmd).Reverse())
                .Concat(BitConverter.GetBytes(commandValue).Reverse())
                .ToList();
            var crc = ModRTU_CRC(sequence.ToArray(), sequence.Count);
            sequence.AddRange(BitConverter.GetBytes(crc));
            return sequence;
        }
        static UInt16 ModRTU_CRC(byte[] buf, int len)
        {
            UInt16 crc = 0xFFFF;

            for (int pos = 0; pos < len; pos++)
            {
                crc ^= (UInt16)buf[pos];          // XOR byte into least sig. byte of crc

                for (int i = 8; i != 0; i--)
                {    // Loop over each bit
                    if ((crc & 0x0001) != 0)
                    {      // If the LSB is set
                        crc >>= 1;                    // Shift right and XOR 0xA001
                        crc ^= 0xA001;
                        //crc ^= 0x8005;
                    }
                    else                            // Else LSB is not set
                        crc >>= 1;                    // Just shift right
                }
            }
            // Note, this number has low and high bytes swapped, so use it accordingly (or swap bytes)
            return crc;
        }

        public void SetLightDimmingLevel()
        {
            ReadTemperatureEnabled = false;
            SetSwitchLedLight(false, false, LightStatus, false, !LightStatus);

            _temperatureAndLightsRadioStream.Write(new byte[] { 0x8E, 0x2C, 0x80, 0x02, (byte)(LightStatus ? 0x01 : 0x00), (byte)DimmingValue });
            _temperatureAndLightsRadioStream.Write(new byte[] { 0x8E, 0x2C, 0x80, 0x02, (byte)(LightStatus ? 0x01 : 0x00), (byte)DimmingValue });
            _temperatureAndLightsRadioStream.Write(new byte[] { 0x8E, 0x2C, 0x80, 0x02, (byte)(LightStatus ? 0x01 : 0x00), (byte)DimmingValue });
            ////_serialPortStream.ReadExisting();//clear buffer
            //byte[] cmByte = { LightStatus ? (byte)0x01 : (byte)0x00, (byte)DimmingValue };
            //SendRS485Command(ProtocolSettings.OptotriacsBoardId, ProtocolSettings.CMDSetLightLevel, BitConverter.ToUInt16(cmByte.Reverse().ToArray()));
            BusyWait(100);

            //Thread.Sleep(500);
            //if (_temperatureAndLightsRadioStream.BytesToRead > 0)
            //{
            //    byte[] data = new byte[_temperatureAndLightsRadioStream.BytesToRead];
            //    _temperatureAndLightsRadioStream.Read(data,0,data.Length);
            //    var demoForm = Application.OpenForms.OfType<Demo>().First();
            //    demoForm.Invoke(() =>
            //    {
            //        demoForm.richTextBoxDebug.AppendText(ToHexString(data)+"\r\n");
            //    });
            //}

            ////ReadResponse();
            ReadTemperatureEnabled = true;
        }

        private void ReadResponse()
        {
            SetMode(RS485Mode.Read);
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.Elapsed.TotalSeconds < 5)
            {
                if (_RS485SerialPortStream.BytesToRead > 0)
                {
                    sw.Restart();
                    while (_RS485SerialPortStream.BytesToRead < 8 && sw.Elapsed.TotalMilliseconds < 500)
                    { }
                    byte[] bytes = new byte[_RS485SerialPortStream.BytesToRead];
                    int readAmount = _RS485SerialPortStream.Read(bytes, 0, bytes.Length);
                    var crc = ModRTU_CRC(bytes, Math.Min(bytes.Length, 6));
                    MessageBox.Show($"Bytes:{bytes.Length}. Hex:{ToHexString(bytes)}. CRC:{ToHexString(BitConverter.GetBytes(crc))}. Cycles:{BitConverter.ToUInt32(bytes, bytes.Length - 4)}");
                    return;
                }
            }

            MessageBox.Show($"No response: Total bytes to read:{_RS485SerialPortStream.BytesToRead}");
        }

        public void TestSwitchLedLight()
        {
            SetSwitchLedLight(true, false, false, false, true);
            return;
            for (byte i = 0; i < 16; i++)
            {
                Thread.Sleep(1000);
                BitArray bitArray = new BitArray(new[] { i });
                SetSwitchLedLight(bitArray.Get(0), bitArray.Get(1), bitArray.Get(2), bitArray.Get(3), i % 2 == 0);
            }
        }

        public void SetSwitchLedLight(bool led1, bool led2, bool led3, bool led4, bool backLightOn)
        {
            ushort cmdValue = 0;
            byte ledVal = 0;
            if (led1)
                ledVal += 1;
            if (led2)
                ledVal += 2;
            if (led3)
                ledVal += 4;
            if (led4)
                ledVal += 8;
            if (backLightOn)
                cmdValue = 1;
            cmdValue = (ushort)((cmdValue << 8) + ledVal);
            SendRS485Command(ProtocolSettings.SwitchId, ProtocolSettings.CMDSwitchLedControl, cmdValue);
        }

        private void SendRS485Command(byte deviceId, ushort cmd, ushort cmdValue)
        {
            BackroundReadsEnabled = false;
            var bytesToWrite = GetCommandSequence(deviceId, cmd, cmdValue);
            SetMode(RS485Mode.Write);
            _RS485SerialPortStream.Write(bytesToWrite.ToArray(), 0, bytesToWrite.Count);
            _RS485SerialPortStream.WaitForDataToBeSentOnWire(bytesToWrite.Count);
            SetMode(RS485Mode.Read);

            //ReadResponse();

            BackroundReadsEnabled = true;
        }

        private void BusyWait(int millisecondsToWait)
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.Elapsed.TotalMilliseconds < millisecondsToWait)
            {
            }
        }

        private static List<byte> FromHexString(string hexString)
        {
            return hexString.Split(' ').Select(c => Convert.ToByte($"{c}", 16)).ToList();
        }

        private void SetMode(RS485Mode mode)
        {
            if (mode == RS485Mode.Write)
                _RS485SerialPortStream.RtsEnable = _RS485SerialPortStream.DtrEnable = false; //set MAX485 transceiver in send mode
            else
                _RS485SerialPortStream.RtsEnable = _RS485SerialPortStream.DtrEnable = true; //set MAX485 transceiver in read mode
        }
        static string ToHexString(byte[] Bytes) => "0x" + BitConverter.ToString(Bytes, 0, Bytes.Length);
        public async Task ReadCOValuesAsync()
        {
            Stopwatch lastTimeDataReceived = Stopwatch.StartNew();
            while (true)
            {
                await Task.Delay(1);
                while (_coSensorRadioStream.BytesToRead > 0)
                {
                    if (lastTimeDataReceived.Elapsed.TotalMilliseconds > 50000)
                    {
                        incommingData = new byte[8];
                    }
                    lastTimeDataReceived.Restart();
                    for (var i = 0; i < incommingData.Length - 1; i++)
                    {
                        incommingData[i] = incommingData[i + 1];
                    }

                    incommingData[incommingData.Length - 1] = (byte)_coSensorRadioStream.ReadByte();
                    HandleCOSensorData(incommingData);
                }
            }
        }
        private void HandleCOSensorData(byte[] bytes)
        {
            if (bytes[0] == 0xed && bytes[1] == 0x05 && bytes[2] == 0x9a && bytes[3] == 0x1f)
            {
                var co2Val = BitConverter.ToUInt16(bytes, 4);
                var batteryVal = BitConverter.ToUInt16(bytes, 6);
                var mainForm = Application.OpenForms.OfType<Demo>().Single();
                mainForm.Invoke(() =>
                {
                    mainForm.lblCOValue.Text = $"Carbon Monoxide:{co2Val}";
                    //mainForm.lblCoBatteryVoltage.Text = $"Battery Voltage:{batteryVal}";
                    mainForm.lblCOReceivedTime.Text = $"Received Time:{DateTime.Now}";
                });
            }
        }
        static bool ReadTemperatureEnabled = true;
        public async Task? ReadTemperatureAndHumidityAsync()
        {
            while (true)
            {
                await Task.Delay(1);
                if (!ReadTemperatureEnabled)
                    continue;
                if (_temperatureAndLightsRadioStream.BytesToRead > 0)
                {
                    await Task.Delay(500);
                }

                byte[] barr = new byte[_temperatureAndLightsRadioStream.BytesToRead];
                _temperatureAndLightsRadioStream.Read(barr, 0, barr.Length);
                if (barr.Length >= 20 && barr[2] == 0xb6)
                {
                    var mcpTemp = ((decimal)BitConverter.ToInt32(barr, 4)) / 100;
                    var bmeTemp = ((decimal)BitConverter.ToInt32(barr, 8)) / 100;
                    var bmeHumidity = ((decimal)BitConverter.ToInt32(barr, 12)) / 100;
                    var voltage = ((decimal)BitConverter.ToInt32(barr, 16)) / 100;

                    var mainForm = Application.OpenForms.OfType<Demo>().Single();
                    mainForm.Invoke(() =>
                    {
                        mainForm.lblTemperature.Text = $"Temperature:{bmeTemp:n2}";
                        mainForm.lblHumidity.Text = $"Humidity:{bmeHumidity:n2}";
                        mainForm.lblBatteryVoltage.Text = $"Battery Voltage:{voltage:n2}";
                        mainForm.lblReceivedTime.Text = $"ReceivedTime:{DateTime.Now}";
                    });
                    //Console.WriteLine($"[{DateTime.Now}][{barr[2]:x2}]MCP:{mcpTemp}|BME_TEMP:{bmeTemp}|BME_HUMIDITY:{bmeHumidity}|Voltage:{voltage}");
                }
            }
        }
    }
}
