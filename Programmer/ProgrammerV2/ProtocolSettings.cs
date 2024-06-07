using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammerV2
{
    internal static class ProtocolSettings
    {
        public const ushort PageSize = 512;
        public const byte OurId = 0xF1;
        public static byte[] MagicPackage = { 0x1a, 0x11, 0xc4, 0x9e };
        public const byte CMD_FLASH_UPDATE = 0xA1;
        public const byte CMD_FLASH_UPDATE_CONFIRMATION = 0xA2;
        public const byte CMD_WRITE_FLASH_PAGE = 0XA3;
        public const byte CMD_PAGE_WRITTEN = 0xA4;
        public const byte CMD_END_FLASH_WRITE = 0XA5;
        public const byte CMD_END_FLASH_CONFIRMATION = 0xA6;

    }
}
