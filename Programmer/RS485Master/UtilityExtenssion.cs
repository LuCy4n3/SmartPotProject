using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RJCP.IO.Ports;

namespace RS485Master
{
    public static class UtilityExtension
    {

        public static void WriteBytes(this SerialPortStream serialPortStream, params UInt16[] dataWords)
        {
            foreach (var word in dataWords)
            {
                serialPortStream.Write(BitConverter.GetBytes(word), 0, 2);
            }
        }
        public static void WriteOneByteArrayAndAdditionalBytes(this SerialPortStream serialPortStream, byte[] firstSetOfBytes, params Byte[] otherBytes)
        {
            serialPortStream.Write(firstSetOfBytes,0, firstSetOfBytes.Length);
            serialPortStream.Write(otherBytes,0,otherBytes.Length);
            serialPortStream.Flush();
        }

        public static void WaitForDataToBeSentOnWire(this SerialPortStream serialPortStream,int bytesCount)
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
        public static string ToHexString(this UInt16 val)
        {
            return ToHexString(BitConverter.GetBytes(val), 2);
        }
        public static string ToHexString(this UInt32 val)
        {
            return ToHexString(BitConverter.GetBytes(val), 4);
        }
        public static string ToHexString(this byte[] Bytes)=>"0x"+BitConverter.ToString(Bytes, 0, Bytes.Length);//.Replace("-","");
        public static string ToHexString(this IEnumerable<byte> Bytes, int length) => ToHexString(Bytes.Take(length).ToArray());
    }
}
