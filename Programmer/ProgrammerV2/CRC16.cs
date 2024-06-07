using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammerV2
{
    internal static class CRC16
    {
        public static UInt16 CRC16_Update(UInt16 crc, byte oneByte)
        {
            int i;
            crc ^= oneByte;
            for (i = 0; i < 8; ++i)
            {
                if ((crc & 1)>0)
                    crc = (UInt16)((crc >> 1) ^ 0xA001);
                else
                    crc = (UInt16)(crc >> 1);
            }
            return crc;
        }

        public static UInt16 ComputeCrc16(IEnumerable<byte> data, UInt16 previousCrc = 0)
        {
            var currentCrc = previousCrc;
            foreach (var byteData in data)
            {
                currentCrc = CRC16_Update(currentCrc, byteData);
            }

            return currentCrc;
        }
    }
}
