using RJCP.IO.Ports;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace SerialTerminal
{
    public class TestTempSensor
    {
        public static void Run()
        {
            //System.IO.Ports.SerialPort sp = new System.IO.Ports.SerialPort("COM7", 2400);
            //sp.DiscardNull = false;
            //sp.Open();
            //sp.

            using SerialPortStream serialPortStream = new SerialPortStream("COM7", 2400);
            //using SerialPortStream serialPortStream = new SerialPortStream("COM5", 9600);
            serialPortStream.ReadBufferSize = 1024;
            serialPortStream.Open();
            //while (true)
            //{
            //    Thread.Sleep(1000);
            //    var str = serialPortStream.ReadExisting();
            //    if (!String.IsNullOrWhiteSpace(str))
            //    {
            //        Console.WriteLine($"[{DateTime.Now}]:{str}");
            //    }
            //}
            //while (true)
            //{
            //    Thread.Sleep(500);
            //    if (serialPortStream.BytesToRead > 0)
            //    {
            //        Console.WriteLine($"[{DateTime.Now}]: {serialPortStream.ReadExisting()}");
            //    }
            //}
            float mcpTemp = 0;
            float bmeTemp = 0;
            float bmeHumidity = 0;
            float voltage = 0;
            Console.WriteLine($"[{DateTime.Now}]: Started");
            while (true)
            {
                Thread.Sleep(100);
                if (serialPortStream.BytesToRead > 0)
                {
                    try
                    {
                        Thread.Sleep(1000);
                        byte[] barr = new byte[serialPortStream.BytesToRead];
                        serialPortStream.Read(barr, 0, barr.Length);
                        var hexStr = ByteArrayToString(barr, barr.Length);
                        //Console.WriteLine($"{DateTime.Now}: 0x" + BitConverter.ToString(barr));
                        //Console.WriteLine($"{DateTime.Now}:{Encoding.UTF8.GetString(barr)}");
                        if (barr.Length >= 8)
                            mcpTemp = (float)BitConverter.ToInt32(barr, 4) / 100;
                        if (barr.Length >= 12)
                            bmeTemp = (float)BitConverter.ToInt32(barr, 8) / 100;
                        if (barr.Length >= 16)
                            bmeHumidity = (float)BitConverter.ToInt32(barr, 12) / 100;
                        if (barr.Length >= 20)
                            voltage = (float)BitConverter.ToUInt32(barr, 16) / 1000;
                        var logMsg =
                            $"[{DateTime.Now}]:Len:{barr.Length} Sensor:{BitConverter.ToString(barr, 2, 1)} MCPTemp:{mcpTemp:n2}. BME Temp:{bmeTemp:n2}. BMEHumidity:{bmeHumidity:n2}. Voltage:{voltage:n2}";
                        Console.WriteLine(logMsg);
                        File.AppendAllLines("log.txt", new[] { logMsg });
                        voltage = mcpTemp = bmeHumidity = bmeTemp = 0;
                        var existing = serialPortStream.ReadExisting();//clear buffer
                        if (!string.IsNullOrEmpty(existing))
                            Console.WriteLine($"[{DateTime.Now}]Existing:{existing.Length}");

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + ex.StackTrace);
                    }
                }
            }
        }

        public static void SendRestartToCentralUnit()
        {
            using SerialPortStream serialPortStream = new SerialPortStream("COM7", 1200);
            serialPortStream.Open();
            byte[] data = new byte[100];
            data[0] = 0xFA;//magic header
            data[1] = 0x76;//magic header
            data[2] = 0x10;//device id
            //data[3] = 0x77;//reset command
            data[3] = 0x02;//CMD set values
            for (var i = 4; i < 10; i++)
            {
                data[i] = 0Xff;
            }

            serialPortStream.ReadExisting();
            serialPortStream.Write(data, 0, 16);
            var sw = Stopwatch.StartNew();
            while (serialPortStream.BytesToRead < 6 && sw.Elapsed.TotalSeconds < 5)
            {
                //Thread.Sleep(1);
            }

            sw.Stop();

            byte[] readData = new byte[100];
            Int32 readAmount = 0;
            if (serialPortStream.BytesToRead > 0)
            {
                readAmount = serialPortStream.Read(readData, 0, readData.Length);
            }
            else
            {
                Console.WriteLine("No data");
            }
        }

        public static void TestReceive()
        {
            using SerialPortStream serialPortStream = new SerialPortStream("COM7", 1200);
            serialPortStream.Open();
            Stopwatch sw = new Stopwatch();
            Console.WriteLine($"[{DateTime.Now}]Started");
            serialPortStream.ReadExisting();
            Byte[] barr = new Byte[100];
            while (true)
            {

                if (serialPortStream.BytesToRead > 0)
                {
                    sw.Restart();
                }

                if (serialPortStream.BytesToRead >= 16)
                {
                    sw.Stop();
                    int readAmount = serialPortStream.Read(barr, 0, serialPortStream.BytesToRead);
                    Console.WriteLine($"Bytes received:{serialPortStream.BytesToRead}     0x{ByteArrayToString(barr, readAmount)} in {sw.Elapsed}");
                    serialPortStream.ReadExisting();
                }
            }
        }

        public static string ByteArrayToString(byte[] ba, int length)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            for (var i = 0; i < length; i++)
            {
                hex.AppendFormat("{0:x2}", ba[i]);
            }
            return hex.ToString();
        }
    }
}
