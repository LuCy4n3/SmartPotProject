using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bifrost.Devices.Gpio;
using Bifrost.Devices.Gpio.Core;
using RJCP.IO.Ports;

namespace Programmer
{
    public class Programmer : IDisposable
    {
        private Int32 totalBytesReceived;
        private SerialPortStream serialPort;
        private InputSettings Settings;
        private Stopwatch sw;
        private byte[] _buffer = new byte[100 * 1024];
        public async Task PerformFlashAsync()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
            Settings = InputSettings.ParseArgsWithInteractiveMenu();
            Console.WriteLine("Received complete settings. Opening serial port");
            serialPort = new SerialPortStream(Settings.SerialPort, (int)Settings.BaudRate);
            serialPort.ReadBufferSize = 64 * 1024;
            serialPort.WriteBufferSize = 64 * 1024;
            serialPort.Open();
            serialPort.ReadExisting();
            //serialPort.RtsEnable = true;
            serialPort.RtsEnable = serialPort.DtrEnable = false;

            //Console.WriteLine("Setting initial channel");
            //await SetChannelAsync(Settings.MainRadioChannel.Value);
            //Console.WriteLine("Sending flash update request to program space." + serialPort.BytesToRead);
            //serialPort.WriteBytes(InputSettings.MAGIC_HEADER, Settings.DeviceId.Value, InputSettings.MSG_FLASH_UPDATE_REQUESTED);
            //now is the right time to read the file
            //sw = Stopwatch.StartNew();
            //while (sw.Elapsed.TotalSeconds < 2)//give 2 seconds to device to send us back the confirmation. If it doesn't send us back the confirmation we will proceed to listen eitherway
            //{
            //    //message should be: 2 bytes magic header + 2 bytes the device id+ 2 bytes msg confirmation
            //    if (serialPort.BytesToRead >= 6)
            //    {
            //        serialPort.Read(_buffer, 0, 6);
            //        serialPort.ReadExisting();//clean buffer
            //        Console.WriteLine("Got from controller:" + _buffer.ToHexString(6));
            //        break;
            //    }

            //    await Task.Delay(1);
            //}
            await SendFileAsync(Settings.FlashBinaryFile);
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Dispose();
        }

        private async Task<byte[]> GetPreparedBytesAsync(string filePath)
        {
            await Task.Yield();
            byte[] fileBytes;
            string fileExtension = Path.GetExtension(filePath);
            if (fileExtension.Equals(".hex", StringComparison.OrdinalIgnoreCase))
            {
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo
                {
                    FileName =
                        @"c:\Program Files (x86)\Arduino\hardware\tools\avr\bin\avr-objcopy.exe",
                    Arguments = $"-I ihex \"{filePath}\" -p -O binary output.bin",
                    WorkingDirectory = Path.GetDirectoryName(filePath)
                };
                process.Start();
                process.WaitForExit();
                fileBytes = await File.ReadAllBytesAsync(Path.Combine(Path.GetDirectoryName(filePath), "output.bin"));
            }
            else if (fileExtension.Equals(".bin", StringComparison.OrdinalIgnoreCase))
                fileBytes = await File.ReadAllBytesAsync(filePath);
            else
                throw new Exception("Invalid file type");

            return fileBytes.Concat(Enumerable.Repeat((byte) 0xFF, 128 - fileBytes.Length % 128)).ToArray();
        }

        private void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            Dispose();
        }

        private async Task SendFileAsync(String filePath)
        {
            Int32 errors = 0;
            byte[] ringBuffer = new byte[9];
            bool programSuccessfull = false;
            while (!programSuccessfull)
            {
                //wait for controller to send us the start command
                Console.WriteLine("Waiting for controller to send us the green light");
                Message lastMessage = null;
                bool validStartMessage = false;
                while (!validStartMessage)
                {
                    lastMessage = await serialPort.ReadMessageAsync(10_000, Settings.DeviceId.Value, InputSettings.MSG_FLASH_UPDATE_READY_TO_BEGIN, ringBuffer);
                    totalBytesReceived += lastMessage.BytesRead;
                    validStartMessage = true;
                    if (lastMessage.HasErrors)
                    {
                        Console.WriteLine("Error:" + lastMessage.Error);
                        validStartMessage = false;
                        //await Task.Delay(2000); //wait one second then clean the buffer
                        //serialPort.ReadExisting();
                    }
                    else
                        Console.WriteLine("Received green light to start." + totalBytesReceived);
                }

                var fileBytes = await GetPreparedBytesAsync(filePath);// await File.ReadAllBytesAsync(filePath);
                Console.WriteLine($"Sending {fileBytes.Length:n0} bytes");
                UInt32 lastPageCrc = 0;
                UInt32 entireFileCrc = 0;
                for (var i = 0; i <= fileBytes.Length; i++)
                {
                    if (i % 128 == 0 && i != 0) //we have a complete page written so we read the response
                    {
                        lastMessage = await serialPort.ReadMessageAsync(2600, Settings.DeviceId.Value, InputSettings.MSG_LAST_PAGE_CRC, null, lastPageCrc);
                        totalBytesReceived += lastMessage.BytesRead;
                        if (lastMessage.HasErrors)
                        {
                            Console.WriteLine(lastMessage.Error);
                            errors++;
                            if (errors > 10)
                            {
                                Console.WriteLine("Too many errors. Application will exit.");
                                serialPort.Dispose();
                                return;
                            }

                            i -= 129;
                            continue;
                        }

                        errors = 0;
                        Console.WriteLine($"Page {i} was sent successful. CRC was ok:{lastMessage.Data.ToHexString()}. TotalB received:{totalBytesReceived}");
                    }

                    if (i % 128 == 0)
                    {
                        if(i!=0)
                            serialPort.WaitForDataToBeSentOnWire(128);
                        if (i == fileBytes.Length) //all done
                        {
                            Console.WriteLine("Writing 0xFFFF");
                            serialPort.Write(BitConverter.GetBytes((UInt16)0xFFFF), 0, 2);
                            break;
                        }

                        lastPageCrc = 0;
                        var pagePosition = BitConverter.GetBytes((UInt16)i);
                        serialPort.Write(pagePosition, 0, 2);
                        lastPageCrc = CRC32.CRC32_1byte(pagePosition, lastPageCrc);
                    }

                    lastPageCrc = CRC32.CRC32_1byte(new byte[1] { fileBytes[i] }, lastPageCrc);
                    serialPort.WriteByte(fileBytes[i]);
                }
                //all bytes sent. Checking entire flash CRC
                foreach (var b in fileBytes)
                {
                    entireFileCrc = CRC32.CRC32_1byte(new byte[1] { b }, entireFileCrc);
                }
                lastMessage = await serialPort.ReadMessageAsync(2600, Settings.DeviceId.Value, InputSettings.MSG_FLASH_COMPLETE, null, entireFileCrc);
                totalBytesReceived += lastMessage.BytesRead;
                if (lastMessage.HasErrors)
                {
                    Console.WriteLine($"Error on entire crc check:{lastMessage.Error}");
                }
                //else
                {
                    Console.WriteLine($"Entire file CRC from device:{lastMessage.Data.ToHexString()}=={entireFileCrc.ToHexString()}");
                    await Task.Delay(500);
                    serialPort.Write(BitConverter.GetBytes(InputSettings.MSG_MASTER_FLASH_CRC_VALID));

                    Console.WriteLine("Yupy!!! Flash complete with no errors!Press enter to exit.");
                    lastMessage = await serialPort.ReadMessageAsync(2600, Settings.DeviceId.Value, InputSettings.MSG_STARTING_MAIN_PROGRAM, null);
                    totalBytesReceived += lastMessage.BytesRead;
                    if (lastMessage.HasErrors)
                    {
                        Console.WriteLine("Error before jumping to program space." + lastMessage.Error);
                    }
                    else
                    {
                        Console.WriteLine("Controller should start executing main program space");
                    }

                    serialPort.Close();
                    serialPort.Dispose();
                    //Console.ReadLine();
                    return;
                }
            }
        }


        public void Dispose()
        {
            try
            {
                //serialPort?.Close();
                serialPort?.Dispose();
            }
            catch { }
        }
    }
}
