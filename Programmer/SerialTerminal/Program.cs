using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Programmer;
using RJCP.IO.Ports;
using RS485Master;

namespace SerialTerminal
{
    class Program
    {
        static async Task AnalyzeIrReadingsAsync()
        {
            string path = @"b:\work\Embedded\Tests\IR_Readings.txt";
            StreamReader streamReader = new StreamReader(path);
            List<List<string>> allReadings = new List<List<string>>();
            while (!streamReader.EndOfStream)
            {
                var oneLine = await streamReader.ReadLineAsync();
                allReadings.Add(oneLine.Split(' ').ToList());
            }

            Dictionary<int, Dictionary<string, int>> analyzedData = new Dictionary<int, Dictionary<string, int>>();
            for (var i = 0; i < allReadings[0].Count; i++)
            {
                analyzedData[i] = new Dictionary<string, int>();
            }


            foreach (var reading in allReadings)
            {
                int idx = 0;
                foreach (var data in reading)
                {
                    if (analyzedData[idx].ContainsKey(data))
                        analyzedData[idx][data] += 1;
                    else
                        analyzedData[idx][data] = 1;
                    idx++;
                }
            }
            var sorted = analyzedData.OrderByDescending(el => el.Value.Count).ToList();
        }

        static async Task Main(string[] args)
        {
            //await AnalyzeIrReadingsAsync();
            //return;
            //await TestOptoAsync();
            int pinNumber = 1;
            string comPort = "COM4  ";
            if (args.Length > 0)
                comPort = args[0];
            if (args.Length > 1)
                pinNumber = int.Parse(args[1]);
            try
            {
                Process.Start("gpio", $"mode {pinNumber} out");
                await Task.Delay(100);
            }
            catch
            {
                Console.WriteLine("GPIO Not present");
            }

            Stopwatch sw = Stopwatch.StartNew();
            double lastTimeOfAByte = sw.Elapsed.TotalMilliseconds;
            byte[] buffer = new byte[1024 * 1024];
            int position = 0;
            Console.WriteLine("1 Test read channels GT25 with HC12");
            Console.WriteLine("2 Test AC commands");
            Console.WriteLine("3 Test dev board from HOPERF");
            Console.WriteLine("A new ATTIny824 thermostats");
            Console.WriteLine("C for cheap lights switch debug");
            Console.WriteLine("D for debug central unit");
            Console.WriteLine("G for debug Gigi");
            Console.WriteLine("H for HEX reader");
            Console.WriteLine("I for inductor channels test");
            Console.WriteLine("L for lights dimming");
            Console.WriteLine("M for multiple voltage readings");
            Console.WriteLine("P for AT commands");
            Console.WriteLine("R Lights switch control with radio bridge");
            Console.WriteLine("S for debug sensors");
            Console.WriteLine("T for text reader");
            Console.WriteLine("U Uart send messages");
            Console.WriteLine("V for voltage and current reading");

            var selectedOption = Console.ReadLine().ToUpper();

            if (selectedOption == "R")
            {
                await TestSwitchesWithRadioBridgeAsync(comPort);
                return;
            }

            if (selectedOption == "I")
            {
                await MultiChannelTestAsync();
                return;
            }

            if (selectedOption == "1")
            {
                await TestReadChannelsGT25AndHc12Async();
                return;
            }

            if (selectedOption == "M")
            {
                try { Process.Start("gpio", $"write {pinNumber} 1"); } catch { }

                await Task.Delay(100);
                await PerformMultipleVoltageReadingsAsync(comPort);
                return;
            }
            if (selectedOption == "C")
            {
                try { Process.Start("gpio", $"write {pinNumber} 1"); } catch { }

                await Task.Delay(100);
                await TestCheapLightSwitch(comPort);
                return;
            }
            if (selectedOption == "L")
            {
                try { Process.Start("gpio", $"write {pinNumber} 1"); } catch { }

                await Task.Delay(100);
                await TestLightsDimmingAsync(comPort);
                //await TestLightSwitch(comPort);
                return;
            }
            if (selectedOption == "D")
            {
                try { Process.Start("gpio", $"write {pinNumber} 1"); } catch { }

                await Task.Delay(100);
                await TestCentralUnitAsync(comPort);
                return;
            }

            if (selectedOption == "G")
            {
                await DebugGigiAsync(comPort);
                return;
            }


            SerialPortStream sport = null;
            int baudRate;
            Console.WriteLine("1 for BAUD 1200");
            Console.WriteLine("2 for BAUD 2400");
            Console.WriteLine("3 for BAUD 9600");
            Console.WriteLine("4 for BAUD 19200");
            Console.WriteLine("5 for BAUD 57600");
            Console.WriteLine("6 for BAUD 115200");
            baudRate = baudRates[int.Parse(Console.ReadLine().Trim()) - 1];
            sport = new SerialPortStream(comPort, baudRate);

            if (selectedOption == "3")
            {
                await TestDevBoardFromHopeRFAsync();
                return;
            }


            if (selectedOption == "U")
            {
                await TestSendUartMessagesAsync(sport);
                return;
            }
            if (selectedOption == "2")
            {
                await TestACCommandsAsync(sport);
                return;
            }
            //sport.Parity = Parity.None;
            //sport.StopBits = StopBits.One;
            //sport.DataBits = 8;
            //sport.Encoding = System.Text.Encoding.UTF7;
            sport.RtsEnable = sport.DtrEnable = true;

            try { Process.Start("gpio", $"write {pinNumber} 0"); } catch { }
            sport.Open();
            sport.RtsEnable = sport.DtrEnable = true;
            //while (true)
            //{
            //    await Task.Delay(1);

            //}

            if (selectedOption == "A")
            {
                await ReadThermostatsAsync(sport);
                return;
            }


            await Task.Delay(500);
            while (true)
            {
                if (selectedOption == "P")
                {
                    sport.RtsEnable = sport.DtrEnable = true;//settings/send mode
                    try { Process.Start("gpio", $"write {pinNumber} 0"); } catch { }
                    var cmd = Console.ReadLine();
                    List<byte> binary = Encoding.UTF8.GetBytes(cmd + "\r\n").ToList();
                    binary.Add(0);
                    sport.Write(binary.ToArray(), 0, binary.Count);
                }
                else
                {
                    //depends if we are for radio or for RS485
                    //sport.RtsEnable = sport.DtrEnable = false;//normal mode for Radio module
                    sport.RtsEnable = sport.DtrEnable = true;//read mode for RS485
                    try { Process.Start("gpio", $"write {pinNumber} 0"); } catch { }
                }
                await Task.Delay(500);
                if (sport.BytesToRead > 0)
                {
                    if (selectedOption == "H" || selectedOption == "S" || selectedOption == "V")
                    {
                        byte[] barr = new byte[sport.BytesToRead];
                        sport.Read(barr, 0, barr.Length);
                        if (selectedOption == "S")
                        {
                            if (barr.Length >= 20)
                            {
                                var mcpTemp = BitConverter.ToInt32(barr, 4);
                                var bmeTemp = BitConverter.ToInt32(barr, 8);
                                var bmeHumidity = BitConverter.ToInt32(barr, 12);
                                var voltage = BitConverter.ToInt32(barr, 16);
                                Console.WriteLine($"[{DateTime.Now}][{barr[2]:x2}]MCP:{mcpTemp}|BME_TEMP:{bmeTemp}|BME_HUMIDITY:{bmeHumidity}|Voltage:{voltage}");
                            }
                            else
                            {
                                Console.WriteLine($"Incomplete data length:{barr.Length}.{ToHexString(barr)}");
                            }
                        }
                        else if (selectedOption == "H")
                        {
                            Console.WriteLine($"[{DateTime.Now}][{barr.Length}]{ToHexString(barr)}");
                        }
                        else if (selectedOption == "V")
                        {
                            if (barr.Length >= 8)
                            {
                                var current = BitConverter.ToUInt16(barr, 2);
                                var voltage = BitConverter.ToUInt16(barr, 4);
                                Console.WriteLine($"[{DateTime.Now}] Current:{current}   Voltage:{voltage}");
                            }
                            else
                            {
                                Console.WriteLine($"Incomplete data length:{barr.Length}.{ToHexString(barr)}");
                            }
                        }
                    }

                    if (selectedOption == "P" || selectedOption == "T")
                    {
                        string str = $"[{DateTime.Now}][{sport.BytesToRead}]{sport.ReadExisting()}";
                        Console.WriteLine(str);
                        await File.AppendAllLinesAsync("log.txt", new[] { str });
                    }
                }

            }
            Thread.Sleep(2000);
            Console.WriteLine(sport.ReadExisting());
            while (true)
            {
                if (sport.BytesToRead >= 16)
                {
                    Thread.Sleep(200);
                    sport.ReadExisting();
                    sport.Write(new byte[] { 0xFA, 0x76, 0x10, 171, 0x02, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 });
                    sport.Flush();
                    Console.WriteLine("Sent response");
                }
                Thread.Sleep(100);
            }
            bool firstTime = true;
            while (true)
            {
                if (firstTime)
                {
                    firstTime = false;
                    while (sport.BytesToRead < 16)
                    {
                        Thread.Sleep(1);
                    }
                }
                if (sport.BytesToRead >= 16)
                {
                    var available = sport.BytesToRead;
                    sport.ReadExisting();
                }
                Thread.Sleep(2000);
            }

            //var s1 = DateTime.Now.ToString();
            //var s2 = DateTime.UtcNow.ToString();
            //var s3 = DateTime.Now.ToUniversalTime().ToString();
            ////Dimming.Run();
            ////return;
            //TestTempSensor.SendRestartToCentralUnit();
            //TestTempSensor.TestReceive();
            TestTempSensor.Run();
            return;
            Console.WriteLine("Hello World!");
            using SerialPortStream sp = new SerialPortStream(args[0], int.Parse(args[1]));
            sp.Open();
            Process.Start("gpio", "mode 1 out");
            Process.Start("gpio", "write 1 0");
            //using GpioController gpioController = new GpioController(PinNumberingScheme.Board, new SunxiDriver(0x01C20800, 0x01F02C00));
            //gpioController.OpenPin(1);
            //gpioController.SetPinMode(1, PinMode.Output);
            //gpioController.Write(1, PinValue.Low);
            Console.WriteLine("Waiting for commands");
            while (true)
            {
                var line = Console.ReadLine();
                sp.WriteLine(line);
                Thread.Sleep(200);
                if (sp.BytesToRead > 0)
                    Console.WriteLine($"Response:{sp.ReadExisting()}");
            }
        }

        private static async Task TestDevBoardFromHopeRFAsync()
        {
            SerialPortStream serialPort = null;
            List<byte> payload = new List<byte>()
            {
                0xAA,//STA1
                0x55,//STA2
                0x10,//CMD_H - CMD_GET_INF
                0x00,//CMD_L - CMD_GET_INF
                0x00,//LEN[0] - length of data
                0x00,//LEN[1] - length of data
                0x00,//PARAM - reserved value 0x00
                0x00,//PARAM - reserved value 0x00
                0x00,//PARAM - reserved value 0x00
                0x00,//PARAM - reserved value 0x00
            };
            var xorByte = payload[0];
            for(var i = 1;i<payload.Count;i++)
            {
                xorByte ^= payload[i];
            }
            payload.Add(xorByte);//add XOR byte to the payload array
            byte[] payLoadWithXor = payload.ToArray();
            //tryout multiple baud rates
            foreach (var baudRate in new int[] { 9600, 2400, 4800, 14400, 38400, 57600, 115200, 19200 })
            {
                serialPort = new SerialPortStream("COM3", baudRate);
                serialPort.Open();  
                //send data over UART
                await serialPort.WriteAsync(payLoadWithXor, 0, payLoadWithXor.Length);
                await Task.Delay(2000);
                var available = serialPort.BytesToRead;
                if(available>0)//this is always 0. There is no response from the device
                {
                    Console.WriteLine("Received data from device");
                }
                serialPort.Close();//close before we try a new baud rate
            }
        }

        private static async Task TestACCommandsAsync(SerialPortStream sport)
        {
            sport.Open();
            byte[] temperatures = { 26, 27, 28, 29, 30 };
            long idx = 0;
            while (true)
            {
                List<byte> data = new List<byte>(new byte[] { 0x1a, 0x11, 0xc4, 0x9e }.Reverse())
                {
                    temperatures[idx%temperatures.Length],
                    0
                };
                await sport.WriteAsync(data.ToArray(), 0, data.Count);
                await Task.Delay(5000);
                idx++;
            }
        }

        private static async Task TestSendUartMessagesAsync(SerialPortStream sport)
        {
            sport.RtsEnable = sport.DtrEnable = true;

            sport.Open();
            sport.RtsEnable = sport.DtrEnable = true;
            int idx = 0;
            while (true)
            {
                var data = Encoding.UTF8.GetBytes($"It works {idx}\r\n");
                await sport.WriteAsync(data, 0, data.Length);
                await Task.Delay(100);
                idx++;
            }
        }

        static ushort crc16_update_internal(ushort crc, byte oneByte)
        {
            byte i;
            crc ^= oneByte;
            for (i = 0; i < 8; ++i)
            {
                if ((crc & 1) > 0)
                    crc = (ushort)((crc >> 1) ^ 0xA001);
                else
                    crc = (ushort)(crc >> 1);
            }
            return crc;
        }

        static ushort crc16_update(IEnumerable<byte> data, int size, ushort currentCrc)
        {
            int idx = 0;
            foreach (var b in data)
            {
                if (idx >= size)
                    return currentCrc;
                idx++;
                //currentCrc = _crc16_update(currentCrc,data[i]);
                currentCrc = crc16_update_internal(currentCrc, b);
            }
            return currentCrc;
        }
        static int[] baudRates = { 1200, 2400, 9600, 19200, 57600, 115200 };
        public static string ToHexString(byte[] Bytes) => "0x" + BitConverter.ToString(Bytes, 0, Bytes.Length);
        public static string ToHexString(IEnumerable<byte> Bytes, int length) => ToHexString(Bytes.Take(length).ToArray());


        private static async Task TestSwitchesWithRadioBridgeAsync(string comPort)
        {
            //comPort = "COM8";
            SerialPortStream serialPort = new SerialPortStream(comPort, 9600);
            //serialPort.DataBits = 8;
            //serialPort.Parity = Parity.None;
            //serialPort.StopBits = StopBits.One;
            serialPort.Open();
            serialPort.RtsEnable = serialPort.DtrEnable = false;//write mode
            await Task.Delay(1000);
            //wait 1 second for initialize
            //byte 1: deviceId
            //byte 2: command - button led notifications ms
            //byte 3: data length (I tried with and without adding data length to the payload)
            //byte 4: data first byte 
            //byte 5: data second byte 

            //byte[] bytesToWrite2 = { 0xff, 3,0,48,0,1};//working - byte 2 : function. byte 3 and 4 is register number, byte 5 and 6 is register counts to read
            //List<byte> bytesToWrite = FromHexString("FF 06 10 0E 00 00").ToList();
            //0x1F-06-10-08-01-02-8F-27
            //0x1F-06-10-08-01-02-8F-27
            //0x1F 06 10 08 01 02 8F 27"
            byte[] bytesToWrite2 = { 0x1f, 0x06, 0x10, 0x08, 0x01, 0x02 };
            var bytesToWriteList = bytesToWrite2.ToList();
            bytesToWriteList.AddRange(BitConverter.GetBytes(ModRTU_CRC(bytesToWrite2, bytesToWrite2.Length)));
            //UInt16 crc = ModRTU_CRC(bytesToWriteArray, bytesToWriteArray.Length); //compute Modbus CRC16 using 0xA001 polynomial and 0xFFFF starting crc
            serialPort.Write(bytesToWriteList.ToArray(), 0, bytesToWriteList.Count);
            //return;
            //byte[] bytesToWrite = FromHexString("02 06 10 03 00 22");//config

            //serialPort.RtsEnable = serialPort.DtrEnable = true; //set MAX485 in receive mode
            await Task.Delay(300);
            //serialPort.RtsEnable = serialPort.DtrEnable = true;
            await Task.Delay(1000);
            var hexData = Programmer.UtilityExtenssion.ToHexString(bytesToWriteList.ToArray());

            while (true)
            {
                //break;
                await Task.Delay(100); //wait for any data
                if (serialPort.BytesToRead > 0)
                {
                    await Task.Delay(500); //wait for any data
                    byte[] barr = new byte[serialPort.BytesToRead]; //serialPort.BytesToRead will give available bytes to read from kernel buffer
                    serialPort.Read(barr, 0, barr.Length);
                    if (barr.Length > 0)
                        Console.WriteLine($"Data length:{barr.Length}. Hex:{ToHexString(barr)}"); //no bytes received and the switch LED timeout is still 1000 ms (default value)
                }
            }
            //while (true)
            {
                serialPort.RtsEnable = serialPort.DtrEnable = false;//set MAX485 transceiver in send mode
                await Task.Delay(100);
                //List<byte> bytesToWrite = FromHexString("02 06 10 03 00 01"); //bytesToWrite.Add(0b00000000);//config//read key
                List<byte> bytesToWrite = FromHexString("FF 06 10 03 00"); bytesToWrite.Add(0b00110110);//config
                //List<byte> bytesToWrite = FromHexString("02 06 10 08"); bytesToWrite.Add(1);/*backlight on*/bytesToWrite.Add(0b00000001);

                //List<byte> bytesToWrite = FromHexString("02 06 10 03 00 10");//sensing

                //bytesToWrite.Add(0b00010110);//all leds are off
                //List<byte> bytesToWrite = FromHexString("FF 06 10 0E 00 00").ToList();
                var bytesToWriteArray = bytesToWrite.ToArray();
                UInt16 crc = ModRTU_CRC(bytesToWriteArray, bytesToWriteArray.Length); //compute Modbus CRC16 using 0xA001 polynomial and 0xFFFF starting crc
                serialPort.Write(bytesToWriteArray, 0, bytesToWriteArray.Length);
                serialPort.Write(BitConverter.GetBytes(crc).ToArray(), 0, 2); //sending the 2 bytes of CRC. I tried reverse the CRC bytes and also normal order
                //at this point the bytes sent (in HEX) are: 0x8D3002FEFE673F  or 0x8D3002FEFE3F67 if reverse for CRC bytes is applied
                UtilityExtension.WaitForDataToBeSentOnWire(serialPort, bytesToWriteArray.Length + 2); //waiting for MAX485 transceiver to send the data on the wire
                serialPort.RtsEnable = serialPort.DtrEnable = true; //set MAX485 in receive mode
                await Task.Delay(100); //wait for any data
                byte[] barr = new byte[serialPort.BytesToRead]; //serialPort.BytesToRead will give available bytes to read from kernel buffer
                serialPort.Read(barr, 0, barr.Length);
                //Console.WriteLine($"Data length:{barr.Length}. Hex:{ToHexString(barr)}"); //no bytes received and the switch LED timeout is still 1000 ms (default value)

                await Task.Delay(100); //wait for any data
                barr = new byte[serialPort.BytesToRead]; //serialPort.BytesToRead will give available bytes to read from kernel buffer
                serialPort.Read(barr, 0, barr.Length);
                if (barr.Length > 0)
                    Console.WriteLine($"Data length:{barr.Length}. Hex:{ToHexString(barr)}"); //no bytes received and the switch LED timeout is still 1000 ms (default value)
            }
            Console.WriteLine("done");
            Console.ReadLine();
        }

        private static async Task ReadThermostatsAsync(SerialPortStream serialPortStream)
        {
            const uint MAGIC_HEADER = 0x1a11c49e;

            byte[] ringBuffer = new byte[15];
            serialPortStream.DtrEnable = serialPortStream.RtsEnable = false;
            while (true)
            {
                await Task.Delay(1);
                if (serialPortStream.BytesToRead > 0)
                {
                    for (var i = 0; i < ringBuffer.Length - 1; i++)
                    {
                        ringBuffer[i] = ringBuffer[i + 1];
                    }

                    ringBuffer[ringBuffer.Length - 1] = (byte)serialPortStream.ReadByte();

                    var magicHeaderCandidate = BitConverter.ToUInt32(ringBuffer.Take(4).Reverse().ToArray());
                    var currentRingBufferHex = UtilityExtenssion.ToHexString(ringBuffer);
                    if (magicHeaderCandidate == MAGIC_HEADER)
                    {
                        var crc16 = crc16_update(ringBuffer, ringBuffer.Length - 2, 0);
                        if (crc16 == BitConverter.ToUInt16(ringBuffer.TakeLast(2).ToArray()))
                        {
                            decimal mcpTemp = (decimal)BitConverter.ToInt16(ringBuffer, 5) / 100;
                            decimal bmeTemp = (decimal)BitConverter.ToInt16(ringBuffer, 7) / 100;
                            decimal bmeHumidity = (decimal)BitConverter.ToInt16(ringBuffer, 9) / 100;
                            decimal batteryVoltage = (decimal)BitConverter.ToInt16(ringBuffer, 11) / 1000;
                            Console.WriteLine($"[{DateTime.Now}][{ringBuffer[4]:X2}]MCPTemp={mcpTemp} BMETemp={bmeTemp} BMEHUmidity={bmeHumidity} BatteryVoltage={batteryVoltage}");
                        }
                    }
                }
            }
        }

        private static async Task TestReadChannelsGT25AndHc12Async()
        {
            SerialPortStream spRadio = new SerialPortStream("COM5", 9600);
            spRadio.RtsEnable = spRadio.DtrEnable = false;//set pin in HIGH mode
            spRadio.Open();
            for (var i = 1; i < 255; i++)
            {
                Console.WriteLine($"[{DateTime.Now}] Testing radio channel {i}");
                spRadio.RtsEnable = spRadio.DtrEnable = true;//set pin in LOW mode
                await Task.Delay(100);
                spRadio.Write($"AT+C{i.ToString().PadLeft(3, '0')}\r\n");
                await Task.Delay(100);
                spRadio.RtsEnable = spRadio.DtrEnable = false;//set pin in HIGH mode
                await Task.Delay(300);
                Console.WriteLine($"[{DateTime.Now}] Channel set response={spRadio.ReadExisting().Replace("\r\n", "").Replace("\n", "")}");
                await Task.Delay(2000);
                if (spRadio.BytesToRead > 0)
                {
                    Console.WriteLine($"[{DateTime.Now}] !!!!!!!!!!!!!Bytes available:{spRadio.BytesToRead} for channel '{i}'. {spRadio.ReadExisting().Replace("\r\n", "").Replace("\n", "")}");
                }
            }
        }

        private static async Task MultiChannelTestAsync()
        {
            SerialPortStream spRadio = new SerialPortStream("COM5", 9600);
            SerialPortStream spWire = new SerialPortStream("COM3", 9600);
            spRadio.RtsEnable = spRadio.DtrEnable = false;//set pin in HIGH mode
            spRadio.Open();
            spWire.Open();
            spRadio.RtsEnable = spRadio.DtrEnable = false;//set pin in HIGH mode
            List<byte> bytesSoFar = new List<byte>();
            while (true)
            {
                await Task.Delay(1);
                var wireLine = spWire.ReadLine();
                if (wireLine.Length > 10)
                {

                }
                wireLine = wireLine.Replace("\r", "");
                var idx = wireLine.IndexOf("AT+C");
                if (idx >= 0)
                {
                    Console.WriteLine("------------------------------------");
                    var newChannel = wireLine.Substring(idx + 4);
                    Console.WriteLine($"[{DateTime.Now}] New channel:{newChannel}");
                    spRadio.RtsEnable = spRadio.DtrEnable = true;//set pin in LOW mode
                    spRadio.ReadExisting();
                    //spRadio.BaudRate = 9600;
                    await Task.Delay(100);
                    spRadio.Write($"AT+C{int.Parse(newChannel).ToString().PadLeft(3, '0')}\r\n");
                    await Task.Delay(500);
                    //spRadio.BaudRate = 1200;
                    Console.WriteLine($"[{DateTime.UtcNow}]Response from radio setup:{spRadio.ReadExisting().Replace("\r\n", "").Replace("\n", "")}");

                    spRadio.RtsEnable = spRadio.DtrEnable = false;//set pin in HIGH mode
                }
                else
                {
                    if (spRadio.BytesToRead > 0)
                    {
                        var radioData = spRadio.ReadExisting();
                        Console.WriteLine($"[{DateTime.Now}]!!!! Radio data[{radioData.Length}]:{radioData.Replace("\r\n", "").Replace("\n", "")}");
                    }
                    else
                    {
                        Console.WriteLine($"[{DateTime.Now}]No radio data. Wire data[{wireLine.Length}]:{wireLine}");
                    }
                }
            }
        }

        private static async Task TestOptoAsync2()
        {
            int baud = 9600;
            string comPort = "COM3";
            SerialPortStream sps = new SerialPortStream(comPort, baud);
            sps.Open();

            SerialPortStream repeater = new SerialPortStream("COM17", baud);
            repeater.Open();
#pragma warning disable CS4014
            Task.Run(async () =>
#pragma warning restore CS4014
            {
                while (true)
                {
                    if (repeater.BytesToRead > 0)
                    {
                        byte[] barr = new byte[repeater.BytesToRead];
                        repeater.Read(barr, 0, barr.Length);
                        repeater.Write(barr, 0, barr.Length);
                    }
                    else
                    {
                        await Task.Delay(1);
                    }
                }

            });
            await Task.Delay(1000);
            while (true)
            {
                var data = Guid.NewGuid().ToString();
                sps.Write(data);
                await Task.Delay(1000);
                string response = null;
                if (sps.BytesToRead > 0)
                    response = sps.ReadExisting();

                Console.WriteLine(
                    $"[{DateTime.Now}] Date written:'{data}'. Data received {response}. Are equal:{response == data}");
            }
        }

        private static async Task TestOptoAsync()
        {
            int baud = 9600;
            string comPort = "COM3";
            SerialPortStream sps = new SerialPortStream(comPort, baud);
            sps.Open();

            SerialPortStream repeater = new SerialPortStream("COM17", baud);

            //repeater.Open();
#pragma warning disable CS4014
            Task.Run(async () =>
#pragma warning restore CS4014
            {
                return;
                while (true)
                {
                    if (repeater.BytesToRead > 0)
                    {
                        byte[] barr = new byte[repeater.BytesToRead];
                        repeater.Read(barr, 0, barr.Length);
                        repeater.Write(barr, 0, barr.Length);
                    }
                    else
                    {
                        await Task.Delay(1);
                    }
                }

            });
            await Task.Delay(1000);
            while (true)
            {
                var data = Guid.NewGuid().ToByteArray()[0];
                sps.WriteByte(data);
                await Task.Delay(1000);
                byte[] response = new byte[1];
                if (sps.BytesToRead > 0)
                    sps.Read(response, 0, 1);
                if (sps.BytesToRead > 0)
                    sps.ReadExisting();

                Console.WriteLine(
                    $"[{DateTime.Now}] Date written:'{Convert.ToString(data, 2).PadLeft(8, '0')}'. Data received [1]:{Convert.ToString(response[0], 2).PadLeft(8, '0')}. Are equal:{response[0] == data}");
            }
        }

        private static async Task PerformMultipleVoltageReadingsAsync(string comPort)
        {
            Console.WriteLine("Press enter to start");
            Console.ReadLine();
            SerialPortStream serialPort = new SerialPortStream(comPort, 250_000);
            //serialPort.DataBits = 8;
            //serialPort.Parity = Parity.None;
            //serialPort.StopBits = StopBits.One;
            serialPort.Open();
            serialPort.RtsEnable = serialPort.DtrEnable = true;//st settings pin LOW 
            byte[] buffer = new byte[4];
            await Task.Delay(1000);
            List<UInt16> values = new List<ushort>(100_000);

            while (true)
            {
                while (serialPort.BytesToRead > 0)
                {
                    for (var i = 0; i < buffer.Length - 1; i++)
                    {
                        buffer[i] = buffer[i + 1];
                    }

                    buffer[buffer.Length - 1] = (byte)serialPort.ReadByte();
                    if (buffer[0] == 0xfe && buffer[1] == 0xdc)
                    {
                        var val = BitConverter.ToUInt16(buffer, 2);
                        if (val > 100 && val < 5000)
                            values.Add(val);
                        else
                            Console.WriteLine("Out of range");
                        if (values.Count > 20_000)
                        {
                            StreamWriter sw = new StreamWriter("volktage_values.txt", false);
                            foreach (var v in values)
                            {
                                await sw.WriteLineAsync(v.ToString());
                            }

                            await sw.FlushAsync();
                            sw.Close();
                            await sw.DisposeAsync();
                            Console.WriteLine("DONE!");
                            Console.ReadLine();
                            return;
                        }
                        if (values.Count % 10_000 == 0)
                        {
                            string str = $"[{DateTime.Now}][{serialPort.BytesToRead}]{BitConverter.ToUInt16(buffer, 2)}";
                            Console.WriteLine(str);
                        }
                    }
                }
                await Task.Delay(1);
            }
        }

        private static async Task TestLightsDimmingAsync(string comPort)
        {
            SerialPortStream serialPort = new SerialPortStream(comPort, 9600);
            //serialPort.DataBits = 8;
            //serialPort.Parity = Parity.None;
            //serialPort.StopBits = StopBits.One;
            serialPort.Open();
            serialPort.RtsEnable = serialPort.DtrEnable = false;//st settings pin HIGH 
            await Task.Delay(1000);
            //first 2 bytes = magic package header.
            //3 - is deviceId
            //4 - CMD
            byte[] readStatusesCmd = { 0x8E, 0x2C, 0x80, 0x04, 1, 2, 3, 4 };

            byte[] setDataCmd = { 0x8E, 0x2C, 0x80, 0x02, 0, 0 };
            while (true)
            {
                Console.Write("Enter dimming value:");

                var dimmingValue = byte.Parse(Console.ReadLine());

                //dimmingValue = 30;
                setDataCmd[4] = 1;
                setDataCmd[5] = dimmingValue;
                Console.WriteLine($"[{DateTime.Now}] Sending set lights cmd .Dimming:{dimmingValue}. Cmd VAL:{UtilityExtension.ToHexString(setDataCmd)}");
                serialPort.Write(setDataCmd);
                await Task.Delay(1000);
                //serialPort.ReadExisting();

                Console.WriteLine($"[{DateTime.Now}] Sending retrieve statuses cmd");
                serialPort.Write(readStatusesCmd);

                Stopwatch sw = Stopwatch.StartNew();
                while (sw.Elapsed.TotalSeconds < 3)
                {
                    if (serialPort.BytesToRead > 0)
                    {
                        await Task.Delay(1000);
                        byte[] receivedData = new byte[serialPort.BytesToRead];
                        serialPort.Read(receivedData, 0, receivedData.Length);
                        Console.WriteLine($"[{DateTime.Now}] Received Data:{UtilityExtension.ToHexString(receivedData)}");
                        break;
                    }
                    await Task.Delay(100);
                }

                if (sw.Elapsed.TotalSeconds > 3)
                    Console.WriteLine("Timeout receiving any data");
                await Task.Delay(100);
                //serialPort.ReadExisting();
            }

        }

        private static async Task TestCheapLightSwitch(string comPort)
        {
            SerialPortStream serialPort = new SerialPortStream(comPort, 9600);
            //serialPort.DataBits = 8;
            //serialPort.Parity = Parity.None;
            //serialPort.StopBits = StopBits.One;
            serialPort.Open();
            serialPort.RtsEnable = serialPort.DtrEnable = false;//set MAX485 transceiver in send mode
            await Task.Delay(1000);
            //wait 1 second for initialize
            //byte 1: deviceId
            //byte 2: command - button led notifications ms
            //byte 3: data length (I tried with and without adding data length to the payload)
            //byte 4: data first byte 
            //byte 5: data second byte 

            //byte[] bytesToWrite = { 141, 3,0,48,0,1};//working - byte 2 : function. byte 3 and 4 is register number, byte 5 and 6 is register counts to read

            //byte[] bytesToWrite = FromHexString("02 06 10 03 00 22");//config

            //serialPort.RtsEnable = serialPort.DtrEnable = true; //set MAX485 in receive mode
            while (true)
            {
                //break;
                await Task.Delay(100); //wait for any data
                if (serialPort.BytesToRead > 0)
                {
                    await Task.Delay(500); //wait for any data
                    byte[] barr = new byte[serialPort.BytesToRead]; //serialPort.BytesToRead will give available bytes to read from kernel buffer
                    serialPort.Read(barr, 0, barr.Length);
                    if (barr.Length > 0)
                        Console.WriteLine($"Data length:{barr.Length}. Hex:{ToHexString(barr)}"); //no bytes received and the switch LED timeout is still 1000 ms (default value)
                }
            }
            //while (true)
            {
                serialPort.RtsEnable = serialPort.DtrEnable = false;//set MAX485 transceiver in send mode
                await Task.Delay(100);
                //List<byte> bytesToWrite = FromHexString("02 06 10 03 00 01"); //bytesToWrite.Add(0b00000000);//config//read key
                List<byte> bytesToWrite = FromHexString("FF 06 10 03 00"); bytesToWrite.Add(0b00110110);//config
                //List<byte> bytesToWrite = FromHexString("02 06 10 08"); bytesToWrite.Add(1);/*backlight on*/bytesToWrite.Add(0b00000001);

                //List<byte> bytesToWrite = FromHexString("02 06 10 03 00 10");//sensing

                //bytesToWrite.Add(0b00010110);//all leds are off
                //List<byte> bytesToWrite = FromHexString("FF 06 10 0E 00 00").ToList();
                var bytesToWriteArray = bytesToWrite.ToArray();
                UInt16 crc = ModRTU_CRC(bytesToWriteArray, bytesToWriteArray.Length); //compute Modbus CRC16 using 0xA001 polynomial and 0xFFFF starting crc
                serialPort.Write(bytesToWriteArray, 0, bytesToWriteArray.Length);
                serialPort.Write(BitConverter.GetBytes(crc).ToArray(), 0, 2); //sending the 2 bytes of CRC. I tried reverse the CRC bytes and also normal order
                //at this point the bytes sent (in HEX) are: 0x8D3002FEFE673F  or 0x8D3002FEFE3F67 if reverse for CRC bytes is applied
                UtilityExtension.WaitForDataToBeSentOnWire(serialPort, bytesToWriteArray.Length + 2); //waiting for MAX485 transceiver to send the data on the wire
                serialPort.RtsEnable = serialPort.DtrEnable = true; //set MAX485 in receive mode
                await Task.Delay(100); //wait for any data
                byte[] barr = new byte[serialPort.BytesToRead]; //serialPort.BytesToRead will give available bytes to read from kernel buffer
                serialPort.Read(barr, 0, barr.Length);
                //Console.WriteLine($"Data length:{barr.Length}. Hex:{ToHexString(barr)}"); //no bytes received and the switch LED timeout is still 1000 ms (default value)

                await Task.Delay(100); //wait for any data
                barr = new byte[serialPort.BytesToRead]; //serialPort.BytesToRead will give available bytes to read from kernel buffer
                serialPort.Read(barr, 0, barr.Length);
                if (barr.Length > 0)
                    Console.WriteLine($"Data length:{barr.Length}. Hex:{ToHexString(barr)}"); //no bytes received and the switch LED timeout is still 1000 ms (default value)
            }
            Console.WriteLine("done");
            Console.ReadLine();
        }

        private static List<byte> FromHexString(string hexString)
        {
            return hexString.Split(' ').Select(c => Convert.ToByte($"{c}", 16)).ToList();
        }

        private static async Task TestLightSwitch(string comPort)
        {
            SerialPortStream serialPort = new SerialPortStream(comPort, 19200);
            serialPort.DataBits = 8;
            //serialPort.Parity = Parity.None;
            //serialPort.StopBits = StopBits.One;
            serialPort.Open();
            serialPort.RtsEnable = serialPort.DtrEnable = false;//set MAX485 transceiver in send mode
            await Task.Delay(1000);
            //wait 1 second for initialize
            //byte 1: deviceId
            //byte 2: command - button led notifications ms
            //byte 3: data length (I tried with and without adding data length to the payload)
            //byte 4: data first byte 
            //byte 5: data second byte 

            //byte[] bytesToWrite = { 141, 3,0,48,0,1};//working - byte 2 : function. byte 3 and 4 is register number, byte 5 and 6 is register counts to read
            byte[] bytesToWrite = { 141, 6, 0, 48, 0x13, 0x88 };
            UInt16 crc = ModRTU_CRC(bytesToWrite, bytesToWrite.Length);//compute Modbus CRC16 using 0xA001 polynomial and 0xFFFF starting crc
            serialPort.Write(bytesToWrite, 0, bytesToWrite.Length);
            serialPort.Write(BitConverter.GetBytes(crc).ToArray(), 0, 2);//sending the 2 bytes of CRC. I tried reverse the CRC bytes and also normal order
            //at this point the bytes sent (in HEX) are: 0x8D3002FEFE673F  or 0x8D3002FEFE3F67 if reverse for CRC bytes is applied
            UtilityExtension.WaitForDataToBeSentOnWire(serialPort, bytesToWrite.Length + 2);//waiting for MAX485 transceiver to send the data on the wire
            serialPort.RtsEnable = serialPort.DtrEnable = true;//set MAX485 in receive mode
            await Task.Delay(1000);//wait for any data
            byte[] barr = new byte[serialPort.BytesToRead];//serialPort.BytesToRead will give available bytes to read from kernel buffer
            serialPort.Read(barr, 0, barr.Length);
            Console.WriteLine($"Data length:{barr.Length}. Hex:{ToHexString(barr)}");//no bytes received and the switch LED timeout is still 1000 ms (default value)

            await Task.Delay(1000);//wait for any data
            barr = new byte[serialPort.BytesToRead];//serialPort.BytesToRead will give available bytes to read from kernel buffer
            serialPort.Read(barr, 0, barr.Length);
            Console.WriteLine($"Data length:{barr.Length}. Hex:{ToHexString(barr)}");//no bytes received and the switch LED timeout is still 1000 ms (default value)
            Console.ReadLine();
        }
        //// Compute the MODBUS RTU CRC
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
        private static async Task DebugGigiAsync(string comPort)
        {
            //int[] baudRatesToTry = { 1200, 9600 };
            //foreach (var baudToTry in baudRatesToTry)
            //{
            //    await using SerialPortStream sp = new SerialPortStream(comPort, baudToTry);
            //    sp.Open();
            //    sp.RtsEnable = true;
            //    await Task.Delay(200);
            //    sp.Write("AT+B2400");
            //    await Task.Delay(200);
            //}
            SerialPortStream serialPort = new SerialPortStream(comPort, 2400);
            //serialPort.Open();
            //await Task.Delay(200);
            //serialPort.RtsEnable = true;
            //await Task.Delay(200);
            //serialPort.Write("AT+C044");
            //await Task.Delay(200);
            //serialPort.RtsEnable = false;
            //await Task.Delay(200);
            //serialPort.Dispose();
            //await Task.Delay(200);
            //serialPort = new SerialPortStream(comPort, 2400);
            serialPort.RtsEnable = serialPort.DtrEnable = false;
            serialPort.Open();
            serialPort.RtsEnable = serialPort.DtrEnable = false;
            //serialPort.ReadExisting();//clear buffer
            serialPort.Write(new byte[] { 0x24, 0xfd, 0xc5, 0xa1 }, 0, 4);//magic package
            serialPort.WriteByte(0xC3);//device id 
            serialPort.WriteByte(0xAA);//our device id
            serialPort.WriteByte(0xE0);//CMD_SET_RELAYS/CMD_SET_ACTUATORS
            var message = new byte[] { 6, 10, 1, 1, 0, 0, 1 };//first byte is msg length
            serialPort.Write(message, 0, message.Length);
            //serialPort.Flush();
            Thread.Sleep(1000);
            if (serialPort.BytesToRead > 0)
            {
                byte[] barr = new byte[serialPort.BytesToRead];
                serialPort.Read(barr, 0, barr.Length);
                Console.WriteLine($"[{DateTime.Now}][{barr.Length}]{UtilityExtension.ToHexString(barr)}");
            }
        }

        private static async Task<int> AutoDetectBaudRateAsync(string comPort)
        {
            foreach (var baudToTry in baudRates)
            {
                await using SerialPortStream sp = new SerialPortStream(comPort, baudToTry);
                sp.Open();
                sp.RtsEnable = sp.DtrEnable = true;
                await Task.Delay(800);
                sp.Write("AT+RX");
                await Task.Delay(800);
                var response = sp.ReadExisting();
                if (response.StartsWith("OK+"))
                {
                    var idx1 = response.IndexOf("OK+B") + 4;
                    var idx2 = response.IndexOf("\r\n", idx1);
                    string baud = response.Substring(idx1, idx2 - idx1);
                    return int.Parse(baud);
                }
            }
            throw new Exception("Cannot detect baud rate");
        }


        private static async Task TestCentralUnitAsync(string comPort)
        {
            CentralUnitManager centralUnitManager = new CentralUnitManager();
            await centralUnitManager.StartAsync(comPort);
        }
    }
}
