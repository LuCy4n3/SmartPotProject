// See https://aka.ms/new-console-template for more information

namespace ProgrammerV2;

class Program
{
    static async Task Main(string[] args)
    {
        //await RS485Switches.PerformInitialSetupAsync();
        //return;

        int baudRate;
        string comPort;
        string filePath;
        string gpioSetPin = null;
        byte deviceId;
        var fullArg = args.SingleOrDefault(el => el.StartsWith("SerialPort=", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrWhiteSpace(fullArg))
        {
            Console.Write("Enter com port:");
            comPort = Console.ReadLine();
            Console.WriteLine();
        }
        else { comPort = fullArg.Split('=').Last().Trim(); }

        fullArg = args.SingleOrDefault(el => el.StartsWith("BaudRate=", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrWhiteSpace(fullArg))
        {
            Console.Write("BAUD Rate:");
            baudRate = int.Parse(Console.ReadLine());
            Console.WriteLine();
        }
        else { baudRate = int.Parse(fullArg.Split('=').Last().Trim()); }

        fullArg = args.SingleOrDefault(el => el.StartsWith("FilePath=", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrWhiteSpace(fullArg))
        {
            Console.Write("File path:");
            filePath = Console.ReadLine().Trim();
            Console.WriteLine();
        }
        else { filePath = fullArg.Split('=').Last().Replace("\"", "").Replace("'", "").Trim(); }

        fullArg = args.SingleOrDefault(el => el.StartsWith("GPIOPin=", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrWhiteSpace(fullArg))
        {
            Console.Write("Use GPIO pin for set? [N/Y] default is No:");
            var useGpioPin = Console.ReadLine().Trim();
            if (useGpioPin.Equals("Y", StringComparison.OrdinalIgnoreCase) || useGpioPin.Equals("Yes", StringComparison.OrdinalIgnoreCase))
            {
                Console.Write("Enter GPIO Pin:");
                gpioSetPin = Console.ReadLine().Trim();
                Console.WriteLine();
            }
            
        }
        else { gpioSetPin = fullArg.Split('=').Last().Trim(); }

        fullArg = args.SingleOrDefault(el => el.StartsWith("DeviceId=", StringComparison.OrdinalIgnoreCase));
        string deviceIdStr;
        if (string.IsNullOrWhiteSpace(fullArg))
        {
            Console.Write("Device ID:");
            deviceIdStr = Console.ReadLine().Trim();
            Console.WriteLine();
        }
        else { deviceIdStr = fullArg.Split('=').Last().Trim(); }

        if(deviceIdStr.StartsWith("0x",StringComparison.OrdinalIgnoreCase))
        {
            //try hex parsing
            deviceId = Convert.ToByte(deviceIdStr.Substring(deviceIdStr.IndexOf("x",StringComparison.OrdinalIgnoreCase)+1), 16);
        }
        else
        {
            deviceId = byte.Parse(deviceIdStr);
        }

        ProgrammingSettings programmingSettings = new ProgrammingSettings()
        {
            BaudRate = baudRate,
            DeviceId = deviceId,
            FilePath = filePath,
            GpioPin = gpioSetPin,
            SerialPort = comPort
        };
        RadioBasedProgrammer radioBasedProgrammer = new RadioBasedProgrammer(programmingSettings);
        await radioBasedProgrammer.ExecuteProgrammingAsync();
    }
}