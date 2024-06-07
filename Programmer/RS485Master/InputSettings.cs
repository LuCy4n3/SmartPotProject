using System;
using System.Collections.Generic;
using System.Text;

namespace RS485Master
{
    public class InputSettings
    {
        public static readonly Byte[] MAGIC_HEADER = { 0xD7, 0xC4, 0x03, 0x04 };
        public static byte SlaveDeviceId = 0x55;
        public static byte OurDeviceId = 0xEE;
        public static byte CMD_RESTART = 0xCC;
        public static byte CMD_OPEN_ZONE = 0xA0;
        public static byte CMD_CLOSE_ZONE = 0xA1;
        public static byte CMD_START_PUMP = 0xA2;
        public static byte CMD_STOP_PUMP = 0xA3;
        public static byte RESPONSE_MESSAGE = 0xD0;
        public static byte OK_RESPONSE = 0xB0;
        public static byte ERR_NO_ZONE_OPENED = 0xB1;
        public static byte ERR_INVALID_ZONE_ID = 0xB2;
        public static byte CMD_GET_STATUSES = 0xA4;
        public static byte ERR_MISSING_TIME_TO_STAY_OPEN_VALUE = 0xB3;

    }
}
