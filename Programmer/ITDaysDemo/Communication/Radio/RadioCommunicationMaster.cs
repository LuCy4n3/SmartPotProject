using RJCP.IO.Ports;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITDaysDemo.Communication.Radio
{
    public class RadioCommunicationMaster
    {
        public static RadioCommunicationMaster Instance { get; }
       
        static RadioCommunicationMaster()
        {
            Instance = new RadioCommunicationMaster();
        }
        private RadioCommunicationMaster()
        {
            _mainSerialPortStream.Open();
        }
        private static byte[] incommingData = new byte[8];
        public async Task BackgroundReadAsync()
        {
            Stopwatch lastTimeDataReceived = Stopwatch.StartNew();
            while (true)
            {
                await Task.Delay(1);
                while (_mainSerialPortStream.BytesToRead > 0)
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

                    incommingData[incommingData.Length - 1] = (byte)_mainSerialPortStream.ReadByte();
                    HandleCommand(incommingData);
                }
            }
        }
        private void HandleCommand(byte[] bytes)
        {
            if (bytes[0] == 0xed && bytes[1] == 0x05 && bytes[2] == 0x9a && bytes[3] == 0x1f)
            {
                var co2Val = BitConverter.ToUInt16(bytes, 4);
                var batteryVal = BitConverter.ToUInt16(bytes, 6);
                var mainForm = Application.OpenForms.OfType<Demo>().Single();
                mainForm.Invoke(() =>
                {
                    mainForm.lblCOValue.Text = $"Carbon Monoxide:{co2Val}";
                    mainForm.lblCoBatteryVoltage.Text = $"Battery Voltage:{batteryVal}";
                    mainForm.lblCOReceivedTime.Text = $"Received Time:{DateTime.Now}";
                });
            }
        }
        public void SetLightDimmingLevel()
        {
            SetSwitchLedLight(false, false, LightStatus, false, !LightStatus);

            _radioSerialPortStream.Write(new byte[] { 0x8E, 0x2C, 0x80, 0x02, (byte)(LightStatus ? 0x01 : 0x00), (byte)DimmingValue });
            _radioSerialPortStream.Write(new byte[] { 0x8E, 0x2C, 0x80, 0x02, (byte)(LightStatus ? 0x01 : 0x00), (byte)DimmingValue });
            _radioSerialPortStream.Write(new byte[] { 0x8E, 0x2C, 0x80, 0x02, (byte)(LightStatus ? 0x01 : 0x00), (byte)DimmingValue });
            ////_serialPortStream.ReadExisting();//clear buffer
            //byte[] cmByte = { LightStatus ? (byte)0x01 : (byte)0x00, (byte)DimmingValue };
            //SendRS485Command(ProtocolSettings.OptotriacsBoardId, ProtocolSettings.CMDSetLightLevel, BitConverter.ToUInt16(cmByte.Reverse().ToArray()));
            BusyWait(100);

            ////ReadResponse();
        }
    }
}
