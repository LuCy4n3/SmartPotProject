using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RJCP.IO.Ports;

namespace RS485Master
{
    public class RS485CMDs
    {
        private readonly string[] _args;
        private readonly byte _deviceId;
        public RS485CMDs(string[] args)
        {
            _args = args;
            Console.WriteLine($"args { string.Join(";", args)}");
            _deviceId = Convert.ToByte(_args[2], 16);
        }

        public async Task StartAsync()
        {
            var comPort = _args[0];
            var pin = _args[1];
            try
            {
                Process.Start("gpio", $"mode {pin} out");
                await Task.Delay(100);
            }
            catch { }
            using (var serialPort = new SerialPortStream(comPort, 9600))
            {
                serialPort.Open();
                while (true)
                {
                    serialPort.RtsEnable = serialPort.DtrEnable = false; //write mode
                    try { Process.Start("gpio", $"write {_args[1]} 1"); } catch { }
                    byte[] bytesToSend = WaitForCommand();
                    serialPort.Write(bytesToSend, 0, bytesToSend.Length);
                    serialPort.WaitForDataToBeSentOnWire(bytesToSend.Length);
                    try { Process.Start("gpio", $"write {_args[1]} 0"); } catch { }
                    serialPort.RtsEnable = serialPort.DtrEnable = true; //read mode
                    Stopwatch sw = Stopwatch.StartNew();
                    while (sw.Elapsed.TotalSeconds < 2 && serialPort.BytesToRead < 5)
                    {
                        await Task.Delay(1);
                    }

                    await Task.Delay(100);
                    byte[] barr = new byte[serialPort.BytesToRead];
                    serialPort.Read(barr, 0, barr.Length);
                    Console.WriteLine($"[{DateTime.Now}][{barr.ToHexString()}][{barr.Length}] {Encoding.UTF8.GetString(barr)}");
                }

            }
        }

        private byte[] WaitForCommand()
        {
            int selectedOption = 0;
            UInt16 secondsForZone = 60;
            while (true)
            {
                Console.WriteLine("[1] Open zone 0");
                Console.WriteLine("[2] Close zone 0");
                Console.WriteLine("[3] Open zone 1");
                Console.WriteLine("[4] Close zone 1");
                Console.WriteLine("[5] Start pump");
                Console.WriteLine("[6] Stop pump");
                Console.WriteLine("[7] Get statuses ");
                Console.WriteLine("[8] Restart device ");
                if (int.TryParse(Console.ReadLine(), out selectedOption) && selectedOption > 0 && selectedOption < 9)
                    break;
            }

            List<byte> allBytes = new List<byte>(InputSettings.MAGIC_HEADER);
            allBytes.Add(_deviceId);
            allBytes.Add(InputSettings.OurDeviceId);
            allBytes.AddRange(new byte[2]);
            int zone = -1;
            if (selectedOption == 1)
            {
                allBytes[6] = InputSettings.CMD_OPEN_ZONE;
                zone = 0;
            }

            if (selectedOption == 2)
            {
                allBytes[6] = InputSettings.CMD_CLOSE_ZONE;
                zone = 0;
            }

            if (selectedOption == 3)
            {
                allBytes[6] = InputSettings.CMD_OPEN_ZONE;
                zone = 1;
            }

            if (selectedOption == 4)
            {
                allBytes[6] = InputSettings.CMD_CLOSE_ZONE;
                zone = 1;
            }

            if (selectedOption == 5)
            {
                allBytes[6] = InputSettings.CMD_START_PUMP;
            }

            if (selectedOption == 6)
            {
                allBytes[6] = InputSettings.CMD_STOP_PUMP;
            }

            if (selectedOption == 7)
            {
                allBytes[6] = InputSettings.CMD_GET_STATUSES;
            }
            if (selectedOption == 8)
            {
                allBytes[6] = InputSettings.CMD_RESTART;
            }
            if (zone >= 0)
            {
                allBytes.Add((byte)zone);
            }

            if (selectedOption == 1 || selectedOption == 3)
            {
                allBytes.AddRange(BitConverter.GetBytes(secondsForZone));
            }

            allBytes[7] = (byte)(allBytes.Count - 8);
            return allBytes.ToArray();
        }
    }
}