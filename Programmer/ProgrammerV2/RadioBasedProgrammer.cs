using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using RJCP.IO.Ports;

namespace ProgrammerV2
{
    internal class RadioBasedProgrammer
    {
        private ProgrammingSettings _programmingSettings;
        private readonly SerialPortStream _serialPort;
        public RadioBasedProgrammer(ProgrammingSettings programmingSettings)
        {
            _programmingSettings = programmingSettings;
            _serialPort = new SerialPortStream(programmingSettings.SerialPort, programmingSettings.BaudRate);
            _serialPort.ReadBufferSize = 64 * 1024;
            _serialPort.WriteBufferSize = 64 * 1024;
        }

        public async Task ExecuteProgrammingAsync()
        {
            List<byte[]> dataPages = await ReadFileAsync();
            _serialPort.Open();
            _serialPort.RtsEnable = _serialPort.DtrEnable = false;//put settings pin to high
            Console.WriteLine($"[{DateTime.Now}]Waiting for device to send back 'ready' confirmation");
            Stopwatch timeoutWatch = Stopwatch.StartNew();
            _serialPort.ReadExisting();//discard any buffer
            while (timeoutWatch.Elapsed.TotalMinutes < 10)
            {
                await SendCommandOnlyAsync(ProtocolSettings.CMD_FLASH_UPDATE);
                if (await WaitForConfirmationResponseAsync(ProtocolSettings.CMD_FLASH_UPDATE_CONFIRMATION))
                {
                    Console.WriteLine($"[{DateTime.Now}] ########## Device is ready. Will start sending data pages ##########");
                    int retryCount = 0;
                    int i;
                    for (i = 0; i < dataPages.Count; i++)
                    {
                        List<byte> bytesToSend = GetHeader(ProtocolSettings.CMD_WRITE_FLASH_PAGE, ProtocolSettings.PageSize + 2)
                            .Concat(BitConverter.GetBytes((UInt16)i)).Concat(dataPages[i]).ToList();

                        bytesToSend.AddRange(BitConverter.GetBytes(CRC16.ComputeCrc16(bytesToSend)));
                        await _serialPort.WriteAsync(bytesToSend.ToArray());

                        if (!await WaitForConfirmationResponseAsync(ProtocolSettings.CMD_PAGE_WRITTEN))
                        {
                            retryCount++;
                            if (retryCount > 5)
                            {
                                Console.WriteLine($"[{DateTime.Now}]Error after retry count 5");
                                break;
                            }

                            Console.WriteLine($"[{DateTime.Now}]Did not received page write confirmation. Will retry operation");
                            i--;
                            continue;
                        }
                        else
                        {
                            retryCount = 0;
                            Console.WriteLine($"[{DateTime.Now}]Written page number:{i+1}/{dataPages.Count}");
                        }
                    }

                    if (i == dataPages.Count)
                    {
                        Console.WriteLine($"[{DateTime.Now}]All page data were sent. Will send finalize command");
                        await SendCommandOnlyAsync(ProtocolSettings.CMD_END_FLASH_WRITE);
                        if (await WaitForConfirmationResponseAsync(ProtocolSettings.CMD_END_FLASH_CONFIRMATION))
                        {
                            Console.WriteLine($"[{DateTime.Now}]Flash update completed");
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"[{DateTime.Now}]Failed to receive final flash end write confirmation");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"[{DateTime.Now}]Timeout waiting for device confirmation. Will retry operation");
                }
            }
        }

        private async Task<List<byte[]>> ReadFileAsync()
        {
            await Task.Yield();
            byte[] fileBytes;
            string fileExtension = Path.GetExtension(_programmingSettings.FilePath);
            if (fileExtension.Equals(".hex", StringComparison.OrdinalIgnoreCase))
            {
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo
                {
                    FileName =
                        @"c:\Program Files (x86)\Atmel\Studio\7.0\toolchain\avr8\avr8-gnu-toolchain\bin\avr-objcopy.exe",
                    Arguments = $"-I ihex \"{_programmingSettings.FilePath}\" -p -O binary output.bin",
                    WorkingDirectory = Path.GetDirectoryName(_programmingSettings.FilePath)
                };
                process.Start();
                await process.WaitForExitAsync();
                fileBytes = await File.ReadAllBytesAsync(Path.Combine(Path.GetDirectoryName(_programmingSettings.FilePath), "output.bin"));
            }
            else if (fileExtension.Equals(".bin", StringComparison.OrdinalIgnoreCase))
                fileBytes = await File.ReadAllBytesAsync(_programmingSettings.FilePath);
            else
                throw new Exception("Invalid file type");

            return fileBytes.Concat(Enumerable.Repeat((byte)0xFF, ProtocolSettings.PageSize - (fileBytes.Length % ProtocolSettings.PageSize)))
                .Chunk(ProtocolSettings.PageSize).ToList();
        }


        private async Task<bool> WaitForConfirmationResponseAsync(byte expectedCmd)
        {
            Stopwatch sw = Stopwatch.StartNew();
            byte[] ringBuffer = new byte[10];
            var magicPackageLength = ProtocolSettings.MagicPackage.Length;
            int totalBytesReceived = 0;
            while (sw.Elapsed.TotalSeconds < 5)
            {
                if (_serialPort.BytesToRead > 0)
                {
                    for (var i = 0; i < ringBuffer.Length - 1; i++)
                    {
                        ringBuffer[i] = ringBuffer[i + 1];
                    }
                    ringBuffer[ringBuffer.Length - 1] = (byte)_serialPort.ReadByte();
                    totalBytesReceived++;


                    bool isValid = true;
                    for (var i = 0; i < magicPackageLength; i++)
                    {
                        if (ProtocolSettings.MagicPackage[i] != ringBuffer[i])
                        {
                            isValid = false;
                            break;
                        }
                    }

                    if (!isValid)
                        continue;
                    if (ringBuffer[magicPackageLength] != ProtocolSettings.OurId)
                    {
                        Console.WriteLine("Invalid our ID");
                        continue;
                    }
                    if (ringBuffer[magicPackageLength + 1] != _programmingSettings.DeviceId)
                    {
                        Console.WriteLine("Invalid source device ID");
                        continue;
                    }
                    if (ringBuffer[magicPackageLength + 2] != expectedCmd)
                    {
                        Console.WriteLine("Invalid expected command");
                        continue;
                    }
                    if (ringBuffer[magicPackageLength + 3] != 0)//data length
                    {
                        Console.WriteLine("Invalid data length");
                        continue;
                    }

                    if (CRC16.ComputeCrc16(ringBuffer.Take(8)) != BitConverter.ToUInt16(ringBuffer, 8))
                    {
                        Console.WriteLine("Invalid CRC");
                        continue;
                    }
                    return true;
                }
                else
                {
                    await Task.Delay(1);
                }
            }

            if (totalBytesReceived > 0)
                Console.WriteLine($"Got total bytes:{totalBytesReceived}");
            return false;
        }

        private List<byte> GetHeader(byte cmd, ushort dataLength)
        {
            return ProtocolSettings.MagicPackage.Concat(new[] { _programmingSettings.DeviceId, ProtocolSettings.OurId, cmd}).Concat(BitConverter.GetBytes(dataLength)).ToList();
        }
        private async Task SendCommandOnlyAsync(byte cmd)
        {
            var data = GetHeader(cmd, 0);
            data.AddRange(BitConverter.GetBytes(CRC16.ComputeCrc16(data)));
            await _serialPort.WriteAsync(data.ToArray());
        }
    }
}
