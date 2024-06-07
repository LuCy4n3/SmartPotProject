using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RJCP.IO.Ports;

namespace SerialTerminal
{
    public class Dimming
    {
        public static void Run()
        {
            using SerialPortStream serialPortStream = new SerialPortStream("COM4", 9600);
            serialPortStream.Open();

            while (true)
            {
                for (var i = 1; i < 100; i+=1)
                {
                    serialPortStream.WriteByte((byte)i);
                    Console.WriteLine(i);
                    Thread.Sleep(300);
                }
            }

            while (true)
            {
                try
                {
                    if (Console.KeyAvailable)
                    {
                        int dimmingVal = int.Parse(Console.ReadLine());
                        serialPortStream.WriteByte((byte)dimmingVal);
                    }
                    if (serialPortStream.BytesToRead > 0)
                    {
                        Console.WriteLine(serialPortStream.ReadExisting());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error:{ex.Message}\r\n{ex.StackTrace}");
                }
                Thread.Sleep(200);
            }
        }
    }
}
