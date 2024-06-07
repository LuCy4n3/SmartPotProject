namespace ITDaysDemo.Communication.RS485
{
    //byte 1: deviceId
    //byte 2: RTU function
    //byte 3 + 4: CMD
    //byte 5+6: CMD value 
    //byte 7+8: ModBus RTU CRC16
    internal class ProtocolSettings
    {
        public const byte SwitchId = 0x02;
        public const byte VoltageAndCurrentMeasureId = 0xBB;
        public const byte RTUFunctionWrite = 0x06;
        public const byte RTUFunctionRead = 0x03;
        public const UInt16 CMDSwitchLedControl = 0x1008;
        public const UInt16 CMDSwitchButtonPressed = 0x100b;
        public const UInt16 CMDReadVoltageAndCurrent = 0xaa01;
        public const UInt16 CMDSetLightLevel = 0x00cc;
        public const UInt16 CMDSetLightOff = 0xccdd;
        public const byte OptotriacsBoardId = 0xcc;
    }
}
