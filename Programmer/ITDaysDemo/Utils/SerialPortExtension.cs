using RJCP.IO.Ports;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITDaysDemo.Utils
{
    internal static class SerialPortExtension
    {
        public static void WaitForDataToBeSentOnWire(this SerialPortStream serialPortStream, int bytesCount)
        {
            double bytesPerMillisecond = serialPortStream.BaudRate;
            bytesPerMillisecond = Math.Ceiling(bytesPerMillisecond / 10 / 1000);//10 bit for one byte, 1000 ms in a second
            double amountToWait = bytesPerMillisecond * bytesCount;
            amountToWait += 1;
            Stopwatch sw = Stopwatch.StartNew();
            //serialPortStream.Flush();
            while (sw.Elapsed.TotalMilliseconds <= amountToWait)//busy wait
            {
            }
        }
    }
}
