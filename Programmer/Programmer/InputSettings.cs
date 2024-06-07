using System;
using System.IO;
using System.Linq;
using RJCP.IO.Ports;

namespace Programmer
{
    public class InputSettings
    {
        private InputSettings()
        {

        }
        public Byte? MainRadioChannel { get; set; }
        public Byte? FlashingRadioChannel { get; set; }
        public String FlashBinaryFile { get; set; }
        public UInt16? DeviceId { get; set; }
        public int BaudRate { get; set; }
        public String SerialPort { get; set; }

        public static readonly UInt16 MAGIC_HEADER =0x38AD;
        public static readonly UInt16 MSG_ERROR_INVALID_FLASH_ADDRESS = 0x0001;
        public static readonly UInt16 MSG_ERROR_TIMEOUT_READING = 0x0002;
        public static readonly UInt16 MSG_PAGE_COMPLETE = 0x0003;
        public static readonly UInt16 MSG_FLASH_COMPLETE = 0x0004;
        public static readonly UInt16 MSG_MASTER_FLASH_CRC_VALID = 0x0005;
        public static readonly UInt16 MSG_FLASH_UPDATE_REQUESTED = 0x0006;
        public static readonly UInt16 MSG_FLASH_UPDATE_READY_TO_BEGIN = 0x0007;
        public static readonly UInt16 MSG_LAST_PAGE_CRC = 0x0008;
        public static readonly UInt16 MSG_STARTING_MAIN_PROGRAM = 0x0009;

        public static InputSettings ParseArgsWithInteractiveMenu()
        {
            Console.WriteLine("Using predefined settings!!!");
            return new InputSettings
            {
                SerialPort = "/dev/ttyUSB1",
                BaudRate = 2400,
                DeviceId = 0x10,//0xA6,
                //DeviceId = 0x23,//0xA6,
                //FlashBinaryFile = @"output.bin",
                //FlashBinaryFile = @"c:\Users\Calin\Documents\Arduino\mcp9808test2\mcp9808test2.ino.arduino_standard.hex",
                //FlashBinaryFile= @"c:\Users\Calin\Documents\Arduino\simple-program\simple-program.ino.arduino_standard.hex",
                //FlashBinaryFile = "TestBootLoaderApp.bin",
                //FlashBinaryFile = @"c:\Users\Calin\Documents\Arduino\test-basic-functionality\test-basic-functionality.ino.arduino_standard.hex",
                //FlashBinaryFile = @"c:\Users\Calin\Documents\Arduino\test-lights\test-lights.ino.arduino_standard.hex",
                //FlashBinaryFile  = @"b:\work\Personal\Embeded\LightsDimming\V1\LightsDimmingV3\LightsDimmingV3\Release\LightsDimmingV3.bin",
                //FlashBinaryFile = @"b:\work\Personal\Embeded\NewThermostat\test-basic-functionality\test-basic-functionality.ino.arduino_standard.hex",
                //FlashBinaryFile = @"b:\work\Personal\Embeded\LightsDimming\V1\LightsDimmingV1\LightsDimmingTestDimming\Release\LightsDimmingTestDimming.hex",
                //FlashBinaryFile = @"b:\work\Personal\Embeded\NewHouse\HeatingSystem-Nelu\CentralUnit\CentralUnit\Release\CentralUnit.hex",
                //FlashBinaryFile = @"b:\work\Personal\Embeded\NewHouse\HeatingSystem-Gigi\CentralUnitGigi\CentralUnitGigi\Release\CentralUnitGigi.hex",
                //FlashBinaryFile = @"B:\work\Personal\Embeded\NewHouse\HeatingSystem-Gigi\CentralUnitGigi\ActuatorsControlUnitGigi\Release\ActuatorsControlUnitGigi.hex",
                FlashBinaryFile = "output.bin",
                FlashingRadioChannel = 1,
                MainRadioChannel = 1
                //SerialPort = "/dev/ttyAMA0"
            };

            InputSettings settings = new InputSettings();
            String readValue = null;
            while (settings.MainRadioChannel == null)
            {
                Console.WriteLine("[1-99]Main radio channel. Enter for default: ");
                readValue = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(readValue))
                {
                    settings.MainRadioChannel = 1;
                    Console.WriteLine($"Using default main radio channel {settings.MainRadioChannel}");
                }
                else
                {
                    if (byte.TryParse(readValue, out var tmp))
                    {
                        settings.MainRadioChannel = tmp;
                    }
                }
            }

            while (settings.FlashingRadioChannel == null)
            {
                Console.WriteLine("[1-99]Flashing radio channel. Enter for default: ");
                readValue = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(readValue))
                {
                    settings.FlashingRadioChannel = 1;
                    Console.WriteLine($"Using default flashing radio channel {settings.FlashingRadioChannel}");
                }
                else
                {
                    if (byte.TryParse(readValue, out var tmp))
                    {
                        settings.FlashingRadioChannel = tmp;
                    }
                }
            }

            var availablePorts = SerialPortStream.GetPortNames().OrderBy(s => s).ToList();
            while (settings.SerialPort == null)
            {
                Console.WriteLine("Enter Serial Port:");
                for (var i = 0; i < availablePorts.Count; i++)
                {
                    Console.WriteLine($"[{i + 1}] {availablePorts[i]}");
                }
                readValue = Console.ReadLine();
                if (int.TryParse(readValue, out var tmp) && tmp > 0 && tmp <= availablePorts.Count)
                {
                    settings.SerialPort = availablePorts[tmp - 1];
                }
            }

            int[] baudRates = { 1200, 2400, 4800, 9600, 19200 };

            while (settings.BaudRate == null)
            {
                Console.WriteLine("Enter baud rate:");
                for (var i = 0; i < baudRates.Length; i++)
                {
                    Console.WriteLine($"[{i + 1}] {baudRates[i]}");
                }
                readValue = Console.ReadLine();
                if (int.TryParse(readValue, out var tmp) && tmp > 0 && tmp <= baudRates.Length)
                {
                    settings.BaudRate = baudRates[tmp - 1];
                }
            }

            while (settings.DeviceId == null)
            {
                Console.WriteLine("Enter device ID:");
                readValue = Console.ReadLine();
                if (UInt16.TryParse(readValue, out var tmp))
                {
                    settings.DeviceId = tmp;
                }
            }
            while (settings.FlashBinaryFile == null)
            {
                Console.WriteLine("Enter flash file location:");
                readValue = Console.ReadLine();
                if (File.Exists(readValue))
                {
                    settings.FlashBinaryFile = readValue;
                }
            }

            return settings;
        }
    }
}
