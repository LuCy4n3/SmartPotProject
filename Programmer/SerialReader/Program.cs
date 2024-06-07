using System;
using System.Threading;
using RJCP.IO.Ports;

namespace SerialReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter COM port");
            string comPort = Console.ReadLine();
            Console.WriteLine("Enter BAUD rate");
            int baud = int.Parse(Console.ReadLine());
            try
            {
                using SerialPortStream serialPort = new SerialPortStream(comPort, baud);
                serialPort.Open();
                while (true)
                {
                    if (serialPort.BytesToRead > 0)
                    {
                        Thread.Sleep(500);
                        byte[] byteArray = new byte[serialPort.BytesToRead];
                        serialPort.Read(byteArray, 0, byteArray.Length);
                        Console.WriteLine($"[{DateTime.Now}] {ToHexString(byteArray)}");
                    }

                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\r\n{ex.StackTrace}");
            }
        }
        public static string ToHexString(byte[] Bytes) => "0x" + BitConverter.ToString(Bytes, 0, Bytes.Length).Replace("-", "");
    }
}
