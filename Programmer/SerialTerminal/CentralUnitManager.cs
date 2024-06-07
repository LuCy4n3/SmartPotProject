using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using RS485Master;

namespace SerialTerminal
{
    public static class SerialPortExtension
    {
        public static void WriteByte(this SerialPort port, byte b) => port.Write(new[] { b }, 0, 1);
    }
    public class CentralUnitManager
    {
        private static byte CMD_RESTART = 0xEE;
        private static byte CMD_GET_ALL = 0xA5;
        public async Task StartAsync(string comPort)
        {
            await Task.Yield();
            using SerialPort sp = new SerialPort(comPort, 2400);
            sp.Open();
            sp.RtsEnable = false;
            Console.WriteLine($"Before send:{sp.BytesToRead}");
            await WriteHeaderAsync(sp, CMD_GET_ALL);
            sp.WriteByte(0x00);
            Stopwatch sw = Stopwatch.StartNew();
            byte[] barr = null;
            while (sw.Elapsed.TotalSeconds < 3 && sp.BytesToRead < 4)
            {
                await Task.Delay(1);
            }

            if (sp.BytesToRead > 0)
            {
                await Task.Delay(500);
                barr = new byte[sp.BytesToRead];
                sp.Read(barr, 0, barr.Length);
                Console.WriteLine($"[{DateTime.Now}]Count:{barr.Length}Received :{barr.ToHexString()}");
            }
            else
            {
                Console.WriteLine("No response received");
            }
        }

        private async Task WriteHeaderAsync(SerialPort sp, byte cmd)
        {
            sp.Write(new byte[] { 0x24, 0xfd, 0xc5, 0xa1 }, 0, 4);//magic package
            sp.WriteByte(0x44);//central unit device id 
            sp.WriteByte(0xAA);//our device id
            sp.WriteByte(cmd);
        }
    }
}
