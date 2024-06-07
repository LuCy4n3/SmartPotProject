using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RJCP.IO.Ports;

namespace RS485Master
{
    public class IrigatiiNelu
    {
        public async Task ReadData(SerialPortStream serialPortStream)
        {
            serialPortStream.RtsEnable = serialPortStream.DtrEnable = true;//read mode
            while (true)
            {
                if (serialPortStream.BytesToRead > 0)
                {
                    Thread.Sleep(100);
                    Console.WriteLine($"[{DateTime.Now}]{serialPortStream.ReadExisting()}");
                }
            }
        }

        byte[] GetStartPumpCmd()
        {
            byte[] bufferCmd = new byte[8];
            Buffer.BlockCopy(InputSettings.MAGIC_HEADER, 0, bufferCmd, 0, 4);
            bufferCmd[4] = InputSettings.SlaveDeviceId;
            bufferCmd[5] = InputSettings.OurDeviceId;
            bufferCmd[6] = InputSettings.CMD_START_PUMP;//cmd
            bufferCmd[7] = 0;//msglength
            return bufferCmd;
        }
        byte[] GetOpenZoneCmd()
        {
            byte[] bufferCmd = new byte[11];
            Buffer.BlockCopy(InputSettings.MAGIC_HEADER, 0, bufferCmd, 0, 4);
            bufferCmd[4] = InputSettings.SlaveDeviceId;
            bufferCmd[5] = InputSettings.OurDeviceId;
            bufferCmd[6] = InputSettings.CMD_OPEN_ZONE;
            bufferCmd[7] = 3;//msglength
            bufferCmd[8] = 0;//zone id
            BitConverter.GetBytes((UInt16)30).CopyTo(bufferCmd, 9);//time in seconds
            return bufferCmd;
        }
        public async Task Execute(bool sendPump)
        {
            using (var serialPort = new SerialPortStream("COM12", 9600))
            {
                serialPort.Open();

                while (true)
                {
                    //ReadData(serialPort);
                    //await Task.Delay(500);
                    Thread.Sleep(500);
                    serialPort.RtsEnable = serialPort.DtrEnable = false; //write mode
                    //while (true);
                        //Thread.Sleep(1000);
                        byte[] bufferCmd = sendPump ? GetStartPumpCmd() : GetOpenZoneCmd();

                    serialPort.Write(bufferCmd, 0, bufferCmd.Length);
                    serialPort.WaitForDataToBeSentOnWire(bufferCmd.Length);

                    serialPort.RtsEnable = serialPort.DtrEnable = true; //read mode
                                                 //while (true)
                    {
                        Stopwatch sw = Stopwatch.StartNew();
                        while (serialPort.BytesToRead < 3 && sw.Elapsed.TotalSeconds < 5) { }

                        //Thread.Sleep(100);
                        await Task.Delay(100);
                        byte[] barr = new byte[serialPort.BytesToRead];
                        serialPort.Read(barr, 0, barr.Length);
                        Console.WriteLine($"[{DateTime.Now}][{barr.ToHexString()}][{barr.Length}] {Encoding.UTF8.GetString(barr)}");
                    }
                    Console.WriteLine("Type any key to restart");
                    sendPump = Console.ReadLine() == "p";
                    var str = serialPort.ReadExisting();
                    if (!String.IsNullOrEmpty(str))
                    {
                        Console.WriteLine($"{str.Length}:{str}");
                    }
                }
            }
        }
    }
}
