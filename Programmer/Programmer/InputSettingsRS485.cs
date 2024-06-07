using System;
using System.Collections.Generic;
using System.Text;

namespace Programmer
{
    public enum MCUType
    {
        ATMega8=1,
        ATMega328 = 2
    }
    public class InputSettingsRS485
    {
        public InputSettingsRS485()
        {
            SerialPort = "COM13";
            BaudRate = 9600;
            SlaveDeviceId = 0xA6;//0xA6,
            MasterDeviceId = 0x11;
            //FlashBinaryFile = @"c:\Work\Personal\Embeded\LightsDimming\test_from_arduino\test_from_arduino.ino_atmega328_8000000L.bin",
            //FlashBinaryFile = @"c:\Users\Calin\Documents\Arduino\mcp9808test2\mcp9808test2.ino.arduino_standard.hex",
            //FlashBinaryFile= @"c:\Users\Calin\Documents\Arduino\simple-program\simple-program.ino.arduino_standard.hex",
            //FlashBinaryFile = "TestBootLoaderApp.bin",
            //FlashBinaryFile = @"c:\Users\Calin\Documents\Arduino\test-basic-functionality\test-basic-functionality.ino.arduino_standard.hex",
            //FlashBinaryFile = @"c:\Users\Calin\Documents\Arduino\test-lights\test-lights.ino.arduino_standard.hex",
            //FlashBinaryFile = @"b:\work\Personal\Embeded\LightsDimming\V1\LightsDimmingV3\LightsDimmingV3\Release\LightsDimmingV3.bin";
            //FlashBinaryFile = @"b:\work\Personal\Embeded\NewThermostat\test-basic-functionality\test-basic-functionality.ino.arduino_standard.hex",
            //FlashBinaryFile = @"b:\work\Personal\Embeded\LightsDimming\V1\LightsDimmingV1\LightsDimmingTestDimming\Release\LightsDimmingTestDimming.hex",
            //FlashBinaryFile = @"b:\work\Personal\Embeded\NewThermostat\CentralUnit\CentralUnit\Release\CentralUnit.hex",
            //FlashBinaryFile = @"b:\work\Personal\Embeded\NewHouse\RS485BootLoader\TestApp\Release\TestApp.hex";
            FlashBinaryFile = @"b:\work\Personal\Embeded\NewHouse\Irigatii-Nelu\Irigatii-Nelu\Release\Irigatii-Nelu.hex";
            MCUType = MCUType.ATMega328;
            //MCUType = MCUType.ATMega8;
        }
        public String FlashBinaryFile { get; set; }
        public Byte SlaveDeviceId { get; set; }
        public Byte MasterDeviceId { get; set; }
        public UInt32? BaudRate { get; set; }
        public String SerialPort { get; set; }
        public MCUType MCUType{ get; set; }

        public byte PageSize
        {
            get
            {
                if (MCUType == MCUType.ATMega8)
                    return 64;
                if (MCUType == MCUType.ATMega328)
                    return 128;
                throw new NotSupportedException("Not configured MCU page size");
            }
        }

        public static readonly Byte[] MAGIC_HEADER = { 0xD7, 0xC4, 0x03, 0x04 };
        public static readonly Byte CMD_WRITEPAGE = 0xA1;
        public static readonly Byte CMD_START_FLASH_WRITE = 0xA2;
        public static readonly Byte  CMD_START_FLASH_WRITE_ACK = 0xC2;
        public static readonly Byte CMD_FINISHED = 0xA5;
        public static readonly Byte MSG_PAGE_CRC = 0xA8;
    }
}
