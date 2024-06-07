using System;

namespace Programmer
{
    public class CRC32
    {
        public static UInt32 CRC32_1byte(byte[] data, UInt32 previousCrc32)
        {
            previousCrc32 = ~previousCrc32;
            for (UInt16 i = 0; i < data.Length; i++)
            {
                previousCrc32 ^= data[i];

                for (var j = 0; j < 8; j++)
                {
                    UInt32 t = ~((previousCrc32 & 1) - 1);
                    previousCrc32 = (previousCrc32 >> 1) ^ (0xEDB88320 & t);
                }
            }

            return ~previousCrc32;
        }
        public static UInt32 CRC32_1byte_Old(byte[] data, UInt32 previousCrc32)
        {
            UInt32 crc = previousCrc32 ^ 0xFFFFFFFF; // same as previousCrc32 ^ 0xFFFFFFFF
            Int32 current = 0;
            Int32 length = data.Length;
            while (length-- != 0)
            {
                byte tmpByte = (byte)(crc % 256);
                byte s = (byte)(tmpByte ^ data[current]);
                current++;
                // Hagai Gold made me aware of this table-less algorithm and send me code
                // polynomial 0xEDB88320 can be written in binary as 11101101101110001000001100100000b
                // reverse the bits (or just assume bit 0 is the first one)
                // and we have bits set at position 0, 1, 2, 4, 5, 7, 8, 10, 11, 12, 16, 22, 23, 26
                // => those are the shift offsets:
                //crc = (crc >> 8) ^
                //       t ^
                //      (t >>  1) ^ (t >>  2) ^ (t >>  4) ^ (t >>  5) ^  // == y
                //      (t >>  7) ^ (t >>  8) ^ (t >> 10) ^ (t >> 11) ^  // == y >> 6
                //      (t >> 12) ^ (t >> 16) ^                          // == z
                //      (t >> 22) ^ (t >> 26) ^                          // == z >> 10
                //      (t >> 23);
                // the fastest I can come up with:
                UInt32 s2 = s;
                UInt32 low = (s2 ^ (s2 << 6)) & 0xFF;
                UInt32 a = (low * ((1 << 23) + (1 << 14) + (1 << 2)));
                crc = (crc >> 8) ^
                      (low * ((1 << 24) + (1 << 16) + (1 << 8))) ^
                      a ^
                      (a >> 1) ^
                      (low * ((1 << 20) + (1 << 12))) ^
                      (low << 19) ^
                      (low << 17) ^
                      (low >> 2);
                if (crc == 0x7CB9A9FA)
                {

                }
                // Hagai's code:
                /*uint32_t t = (s ^ (s << 6)) << 24;
                // some temporaries to optimize XOR
                uint32_t x = (t >> 1) ^ (t >> 2);
                uint32_t y = x ^ (x >> 3);
                uint32_t z = (t >> 12) ^ (t >> 16);
                crc = (crc >> 8) ^
                       t ^ (t >> 23) ^
                       y ^ (y >>  6) ^
                       z ^ (z >> 10);*/
            }
            return crc ^ 0xFFFFFFFF; // same as crc ^ 0xFFFFFFFF
        }
        public static UInt16 CRC16_bytes(byte[] bytes, UInt16 previousCrc)
        {
            var newCrc = previousCrc;
            foreach (var b in bytes)
            {
                newCrc = CRC16_1byte(b, newCrc);
            }
            return newCrc;
        }
        public static UInt16 CRC16_1byte(byte oneByte, UInt16 previousCrc)
        {
            int i;
            previousCrc ^= oneByte;
            for (i = 0; i < 8; ++i)
            {
                if ((previousCrc & 1)>0)
                    previousCrc = (UInt16)((previousCrc >> 1) ^ 0xA001);
                else
                    previousCrc = (UInt16)(previousCrc >> 1);
            }

            return previousCrc;
        }
    }
}
