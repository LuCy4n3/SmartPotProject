using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HexIO;
using RJCP.IO.Ports;

namespace Programmer
{
    
    class Program
    {
        private static int pinNumber;
        //static Delegate ConvertDelegate(Delegate originalDelegate, Type targetDelegateType)
        //{
        //    return Delegate.CreateDelegate(
        //        targetDelegateType,
        //        originalDelegate.Target,
        //        originalDelegate.Method);
        //}

        //private static Stopwatch swTxEmpty = new Stopwatch();
        //static void EvHandler(object obj1, object obj2)
        //{
        //    var evType = obj2.GetType().GetProperty("EventType").GetValue(obj2);
        //    if ((int) evType == 4)
        //    {
        //        swTxEmpty.Stop();
        //    }
        //}
        static async Task Main(string[] args)
        {
            Console.WriteLine("Type Y if is Central Unit Heating device");
            var isHeatingUnit = Console.ReadLine().ToLower().Trim() == "y";
            Stopwatch sw = Stopwatch.StartNew();
            if (isHeatingUnit)
            {
                Console.WriteLine("Enter pin number");
                pinNumber = int.Parse(Console.ReadLine().Trim());
                try
                {
                    Process.Start("gpio", $"mode {pinNumber} out");
                    await Task.Delay(100);
                }
                catch { }

                sw.Restart();
                await SendRestartCommandAsync(pinNumber);
            }

            var settings = InputSettings.ParseArgsWithInteractiveMenu();

            //await TestHexAsync(settings);
            //return;

            var currentHc12Settings = ReadCurrentRadioSettings(settings.SerialPort);
            Console.WriteLine($"Original HC-12 settings : baud {currentHc12Settings.baud} and channel {currentHc12Settings.channel}");
            SetRadioSettingsAsync(isHeatingUnit, settings.BaudRate, "099");
            //SetFU4Async(isHeatingUnit,"099");
            while (sw.Elapsed.TotalSeconds < 1)
            {
                await Task.Delay(1);
            }
            //while (true)
            {
                using (Programmer programmer = new Programmer())
                {
                    await programmer.PerformFlashAsync();
                }

                Console.WriteLine($"Setting back baud {currentHc12Settings.baud} and channel {currentHc12Settings.channel}");
                SetRadioSettingsAsync(true, currentHc12Settings.baud, currentHc12Settings.channel);
                await Task.Delay(1200);
            }
        }

        private static async Task TestHexAsync(InputSettings settings)
        {
            string fileExtension = Path.GetExtension(settings.FlashBinaryFile);
            byte[] avrToolBinaryResult;
            byte[] netLibConverter;
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName =
                    @"c:\Program Files (x86)\Arduino\hardware\tools\avr\bin\avr-objcopy.exe",
                Arguments = $"-I ihex \"{settings.FlashBinaryFile}\" -p -O binary output.bin",
                WorkingDirectory = Path.GetDirectoryName(settings.FlashBinaryFile)
            };
            process.Start();
            process.WaitForExit();
            avrToolBinaryResult = await File.ReadAllBytesAsync(Path.Combine(Path.GetDirectoryName(settings.FlashBinaryFile), "output.bin"));
            await using var fs = File.OpenRead(settings.FlashBinaryFile);
            await using var fsw = File.Create("out.bin");
            IntelHexReader hexReader = new IntelHexReader(fs);
            while (hexReader.Read(out var hexRecord))
            {
                if (hexRecord.RecordType != IntelHexRecordType.EndOfFile)
                {
                    fsw.Seek(hexRecord.Address, SeekOrigin.Begin);
                    fsw.Write(hexRecord.Data.ToArray(), 0, hexRecord.Data.Count);
                    fsw.Flush();
                }
                else
                {

                }
            }

            await fsw.DisposeAsync();
            var inMemoryConverted = await File.ReadAllBytesAsync("out.bin");
            if (inMemoryConverted.Length != avrToolBinaryResult.Length)
            {

            }

            for (var i = 0; i < Math.Min(avrToolBinaryResult.Length, inMemoryConverted.Length);i++)
            {
                if (avrToolBinaryResult[i] != inMemoryConverted[i])
                {

                }
            }
        }

        private static (int baud, string channel) ReadCurrentRadioSettings(string comPort)
        {
            var baudRatesToTry = new[] {1200, 2400, 9600};
            int baud = 0;
            string channel = null;
            foreach (var baudToTry in baudRatesToTry)
            {
                using SerialPortStream sport = new SerialPortStream(comPort, baudToTry);
                sport.Open();
                try { Process.Start("gpio", $"write {pinNumber} 1"); } catch { }
                sport.RtsEnable = sport.DtrEnable = true;
                Thread.Sleep(200);
                sport.WriteLine("AT+RX");
                Thread.Sleep(500);
                var response = sport.ReadExisting();
                if (response.StartsWith("OK"))
                {
                    StringReader sr = new StringReader(response);
                    var line = sr.ReadLine();
                    baud = int.Parse(line.Substring(4));
                    line = sr.ReadLine();
                    channel = line.Substring(5);

                    sport.RtsEnable = sport.DtrEnable = false;
                    Thread.Sleep(200);
                    break;
                }
                sport.RtsEnable = sport.DtrEnable = false;
                Thread.Sleep(200);
            }
            return (baud, channel);
        }

        private static async Task SendRestartCommandAsync(int pin)
        {
            var settings = InputSettings.ParseArgsWithInteractiveMenu();
            SerialPortStream sport;
            SetRadioSettingsAsync(true, 2400, "050");
            try { Process.Start("gpio", $"write {pin} 1"); } catch { }
            await Task.Delay(2000);
            Console.WriteLine("##########Sending reset command");
            sport = new SerialPortStream(settings.SerialPort, 2400);
            sport.Open();
            sport.RtsEnable = sport.DtrEnable = false; //setup mode
            sport.ReadExisting();
            sport.Write(new byte[] { 0x24, 0xfd, 0xc5, 0xa1 }, 0, 4);//magic package
            sport.WriteByte(0x44);//central unit device id 
            sport.WriteByte(0xAA);//our device id
            sport.WriteByte(0xEE);//restart command 
            sport.WriteByte(0x00);//restart command 
            byte[] barr = null;
            Stopwatch sw = Stopwatch.StartNew();

            while (sw.Elapsed.TotalSeconds < 3 && sport.BytesToRead < 4)
            {
                await Task.Delay(1);
            }

            if (sport.BytesToRead > 0)
            {
                await Task.Delay(500);
                barr = new byte[sport.BytesToRead];
                sport.Read(barr, 0, barr.Length);
                Console.WriteLine($"[{DateTime.Now}]Received :{barr.ToHexString()}");
            }
            else
            {
                Console.WriteLine("No response received");
            }
        }

        private static void SetChannel(bool isLinuxGPIO, int baudRateToSet, string channel)
        {
            int delayMs = 200;
            var settings = InputSettings.ParseArgsWithInteractiveMenu();
            using SerialPortStream sport = new SerialPortStream(settings.SerialPort, baudRateToSet);
            sport.Open();
            Thread.Sleep(delayMs);
            sport.RtsEnable = sport.DtrEnable = true; //setup mode
            if (isLinuxGPIO)
                try { Process.Start("gpio", $"write {pinNumber} 0"); } catch { }
            Thread.Sleep(delayMs);
            Console.WriteLine($"Setting channel to {channel}");
            sport.Write("AT+C" + channel);
            Thread.Sleep(delayMs);
            //if (sport.BytesToRead > 0)
            {
                Thread.Sleep(delayMs);
                Console.WriteLine($"AT Response:\r\n{sport.ReadExisting()}");
            }

            sport.RtsEnable = sport.DtrEnable = false;
            if (isLinuxGPIO)
                try { Process.Start("gpio", $"write {pinNumber} 1"); } catch { }
        }
        private static void SetRadioSettingsAsync(bool isLinuxGPIO, int baudRateToSet, string channel)
        {
            var settings = InputSettings.ParseArgsWithInteractiveMenu();
            int[] baudRates = { 2400, 9600,1200 };
            SerialPortStream sport;
            int delayMs = 200;
            if (isLinuxGPIO)
                try { Process.Start("gpio", $"write {pinNumber} 0"); } catch { }
            foreach (var baud in baudRates)
            {
                Console.WriteLine($"Trying baud rate:{baud}");
                sport = new SerialPortStream(settings.SerialPort, baud);
                sport.Open();
                //sport.OpenDirect();
                Thread.Sleep(delayMs);
                sport.RtsEnable = sport.DtrEnable = true; //setup mode
                Thread.Sleep(delayMs);
                sport.Write("AT+B" + baudRateToSet+"\r\n");
                Thread.Sleep(delayMs);
                sport.RtsEnable = sport.DtrEnable = false; //exit setup mode
                //if (sport.BytesToRead > 0)
                {
                    //Thread.Sleep(delayMs);
                    Console.WriteLine($"AT Response:\r\n{sport.ReadExisting()}");
                }
                sport.Dispose();
            }
            if (isLinuxGPIO)
                try { Process.Start("gpio", $"write {pinNumber} 1"); } catch { }
            Thread.Sleep(delayMs);

            //SetChannel(isLinuxGPIO, 9600, channel);
            SetChannel(isLinuxGPIO, baudRateToSet, channel);

            if (isLinuxGPIO)
                try { Process.Start("gpio", $"write {pinNumber} 1"); } catch { }
        }

        private static void SetFU4Async(bool isLinuxGPIO, string channel)
        {
            var settings = InputSettings.ParseArgsWithInteractiveMenu();
            int[] baudRates = { 2400, 9600, 1200 };
            SerialPortStream sport;
            int delayMs = 200;
            if (isLinuxGPIO)
                try { Process.Start("gpio", $"write {pinNumber} 0"); } catch { }
            foreach (var baud in baudRates)
            {
                Console.WriteLine($"Trying baud rate:{baud}");
                sport = new SerialPortStream(settings.SerialPort, baud);
                sport.Open();
                //sport.OpenDirect();
                Thread.Sleep(delayMs);
                sport.RtsEnable = sport.DtrEnable = true; //setup mode
                Thread.Sleep(delayMs);
                sport.Write("AT+FU4\r\n");
                Thread.Sleep(delayMs);
                sport.RtsEnable = sport.DtrEnable = false; //exit setup mode
                //if (sport.BytesToRead > 0)
                {
                    //Thread.Sleep(delayMs);
                    Console.WriteLine($"AT Response:\r\n{sport.ReadExisting()}");
                }
                sport.Dispose();
            }
            if (isLinuxGPIO)
                try { Process.Start("gpio", $"write {pinNumber} 1"); } catch { }
            Thread.Sleep(delayMs);

            //SetChannel(isLinuxGPIO, 9600, channel);
            SetChannel(isLinuxGPIO, 1200, channel);

            if (isLinuxGPIO)
                try { Process.Start("gpio", $"write {pinNumber} 1"); } catch { }
        }

        public static string ToHexString(IEnumerable<byte> bytes, int length) => "0x" + BitConverter.ToString(bytes.ToArray(), 0, length).Replace("-", "");

    }
}
