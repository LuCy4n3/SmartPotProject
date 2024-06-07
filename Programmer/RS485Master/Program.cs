using System;
using System.Diagnostics;
using System.Threading.Tasks;
using RJCP.IO.Ports;

namespace RS485Master
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //SerialPortStream sps = new SerialPortStream("COM14", 1200);
            //sps.Open();
            //Console.WriteLine($"[{DateTime.Now}]Started");
            ////while (true)
            ////{
            ////    await Task.Delay(100);
            ////    if (sps.BytesToRead > 0)
            ////    {
            ////        await Task.Delay(100);
            ////        Console.WriteLine($"[{DateTime.Now}][{sps.BytesToRead}]{sps.ReadExisting()}");
            ////    }
            ////}

            //Stopwatch sw = Stopwatch.StartNew();
            //sps.Write(Guid.NewGuid().ToByteArray());
            //sps.Flush(true);
            //sw.Stop();
            RS485CMDs ir = new RS485CMDs(args);
            while (true)
            {
                await ir.StartAsync();
            }

            Console.WriteLine("Hello World!");
        }
    }
}
