using Bifrost.Devices.Gpio;
using Bifrost.Devices.Gpio.Core;
using RJCP.IO.Ports;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Programmer
{
    public class ProgrammerRS485 : IDisposable
    {
        public ProgrammerRS485()
        {
            Settings = new InputSettingsRS485();
            serialPort = new SerialPortStream(Settings.SerialPort, (int)Settings.BaudRate);
            serialPort.ReadBufferSize = 64 * 1024;
            serialPort.WriteBufferSize = 64 * 1024;
            serialPort.DiscardNull = false;
            serialPort.Open();

        }
        private TimeSpan delayBeforeSwapMode = TimeSpan.FromMilliseconds(70);
        private Int32 totalBytesReceived;
        private SerialPortStream serialPort;
        private InputSettingsRS485 Settings;
        private Stopwatch sw;
        private byte[] _buffer = new byte[1024];
        public async Task PerformFlashAsync()
        {
            Console.WriteLine($"------ MCU {Settings.MCUType} ----------");
            await Task.Delay(1000);
            //Console.WriteLine("Received complete settings. Opening serial port");
            //Console.WriteLine("Port opened waiting for device handshake");
            serialPort.RtsEnable = serialPort.DtrEnable = false;
            //Console.WriteLine("Sending flash update request to program space." + serialPort.BytesToRead);
            serialPort.WriteOneByteArrayAndAdditionalBytes(true, InputSettingsRS485.MAGIC_HEADER, Settings.SlaveDeviceId, Settings.MasterDeviceId, InputSettingsRS485.CMD_START_FLASH_WRITE, 1);

            //await Task.Delay(delayBeforeSwapMode);
            serialPort.RtsEnable = serialPort.DtrEnable = true;//receive mode
            sw = Stopwatch.StartNew();
            while (sw.Elapsed.TotalSeconds < 2)//give 2 seconds to device to send us back the confirmation. If it doesn't send us back the confirmation we will proceed to listen eitherway
            {
                //message should be: 4 bytes magic header + 1 bytes the device id + 1 byte our master device ID 1 byte cmd 1 byte datalength
                if (serialPort.BytesToRead >= 8)
                {
                    await Task.Delay(100);
                    int readAmount = serialPort.Read(_buffer, 0, serialPort.BytesToRead);
                    if (!ValidateResponse(_buffer, InputSettingsRS485.CMD_START_FLASH_WRITE_ACK))
                    {
                        Console.WriteLine("Invalid response from controller:" + _buffer.ToHexString(6));
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Got valid response from controller will start sending pages of binary program. Data from controller:" + _buffer.ToHexString(readAmount));
                        await SendFileAsync(Settings.FlashBinaryFile);
                    }
                    break;
                }

                await Task.Delay(1);
            }

            byte[] allBytes = new byte[serialPort.BytesToRead];
            serialPort.Read(allBytes, 0, allBytes.Length);
            Console.WriteLine($"Timeout reading response. Bytes available:{allBytes.Length}. Hex: {allBytes.ToHexString()}");

        }

        private bool ValidateResponse(byte[] data, byte expectedCmdResponse)
        {
            for (var i = 0; i < 4; i++)
            {
                if (data[i] != InputSettingsRS485.MAGIC_HEADER[i])
                {
                    Console.WriteLine($"Wrong magic package:{data.ToHexString(16)} expected:{InputSettingsRS485.MAGIC_HEADER.ToHexString()}");
                    return false;
                }
            }
            if (data[4] != Settings.SlaveDeviceId)
            {
                Console.WriteLine($"Wrong slave id:{data[4]:X2} expected:{Settings.SlaveDeviceId:X2}");
                return false;
            }
            if (data[5] != Settings.MasterDeviceId)
            {
                Console.WriteLine($"Wrong MasterDeviceId {data[5]:X2} expected:{Settings.MasterDeviceId:X2}");
                return false;
            }
            if (data[6] != expectedCmdResponse)
            {
                Console.WriteLine($"Wrong cmd response {data[6]:X2} expected:{expectedCmdResponse:X2}");
                return false;
            }
            return true;
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

            return fileBytes.Concat(Enumerable.Repeat((byte)0xFF, Settings.PageSize - fileBytes.Length % Settings.PageSize)).ToArray();
        }

        private async Task SendFileAsync(String filePath)
        {
            Int32 errors = 0;
            //wait for controller to send us the start command
            Message lastMessage = null;
            var fileBytes = await GetPreparedBytesAsync(filePath);// await File.ReadAllBytesAsync(filePath);
            Console.WriteLine($"Sending {fileBytes.Length:n0} bytes");
            UInt16 lastPageCrc = 0;
            int errorsCount = 0;
            await Task.Delay(10);
            for (var i = 0; i < fileBytes.Length; i += Settings.PageSize)
            {
                serialPort.RtsEnable = serialPort.DtrEnable = false;//send mode
                var bytesCount = serialPort.WriteOneByteArrayAndAdditionalBytes(false, InputSettingsRS485.MAGIC_HEADER, Settings.SlaveDeviceId, Settings.MasterDeviceId, InputSettingsRS485.CMD_WRITEPAGE, (byte)(Settings.PageSize + 2));
                serialPort.Write(BitConverter.GetBytes((UInt16)i), 0, 2);//page position
                serialPort.Write(fileBytes, i, Settings.PageSize);//write page data
                serialPort.WaitForDataToBeSentOnWire(bytesCount + 2 + Settings.PageSize);
                lastPageCrc = 0;
                lastPageCrc = CRC32.CRC16_bytes(BitConverter.GetBytes((UInt16)i), lastPageCrc);
                lastPageCrc = CRC32.CRC16_bytes(fileBytes.Skip(i).Take(Settings.PageSize).ToArray(), lastPageCrc);

                serialPort.RtsEnable = serialPort.DtrEnable = true;//put in read mode

                if (!await ValidatePageWriteResponse(lastPageCrc))
                {
                    errorsCount++;
                    if (errorsCount > 10)
                    {
                        Console.WriteLine("Too many errors. Will abandon write");
                        return;
                    }
                    i -= Settings.PageSize;//resend page
                }
                Console.WriteLine($"CRC:{lastPageCrc}.Written page:{(i / Settings.PageSize) + 1:n0}/{fileBytes.Length / Settings.PageSize:n0}.");
                //await Task.Delay(delayBeforeSwapMode*10);
                //await Task.Delay(90);
            }


            serialPort.RtsEnable = serialPort.DtrEnable = false;//write mode
            //await Task.Delay(200);
            //sending last finish CMD
            serialPort.WriteOneByteArrayAndAdditionalBytes(true, InputSettingsRS485.MAGIC_HEADER, Settings.SlaveDeviceId, Settings.MasterDeviceId, InputSettingsRS485.CMD_FINISHED, 0);
            serialPort.RtsEnable = serialPort.DtrEnable = true;//read mode
            Byte[] data = new byte[8];
            var readAmount = ReadWithTimeout(data, TimeSpan.FromSeconds(2));
            if (readAmount != data.Length)
            {
                Console.WriteLine($"Did not received enough bytes for final confirmation:{readAmount}. Expected:{data.Length}");
            }
            else
            {
                if (ValidateResponse(data, InputSettingsRS485.CMD_FINISHED))
                {
                    Console.WriteLine("All good! Flash write completed!");
                }
            }
            serialPort.Close();
            await serialPort.DisposeAsync();
            Console.ReadLine();
            return;
        }

        private int ReadWithTimeout(byte[] data, TimeSpan readTimeout)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int readAmount = 0;
            while (sw.Elapsed < readTimeout && readAmount < data.Length)
            {
                if (serialPort.BytesToRead > 0)
                {
                    readAmount += serialPort.Read(data, readAmount, Math.Min(serialPort.BytesToRead, data.Length - readAmount));
                }
                else
                    Thread.Sleep(10);
            }

            return readAmount;
        }

        private async Task<bool> ValidatePageWriteResponse(UInt16 lastPageCrc)
        {
            Stopwatch swForTimeout = Stopwatch.StartNew();
            byte[] data = new byte[10];
            while (swForTimeout.Elapsed.TotalMilliseconds < 1000)
            {
                if (serialPort.BytesToRead >= data.Length)
                {
                    serialPort.Read(data, 0, data.Length);
                    if (!ValidateResponse(data, InputSettingsRS485.MSG_PAGE_CRC))
                        return false;
                    var receivedCrc = BitConverter.ToUInt16(data, 8);
                    if (receivedCrc != lastPageCrc)
                    {
                        Console.WriteLine($"Wrong CRC received:{data.Skip(8).Take(4).ToArray().ToHexString()} expected:{BitConverter.GetBytes(lastPageCrc).ToHexString()}");
                        return false;
                    }

                    //Console.WriteLine($"CRC OK:{lastPageCrc}=={receivedCrc}");
                    return true;
                }
                await Task.Delay(1);
            }
            Console.WriteLine($"Timeout waiting for page write confirmation. Available bytes:{serialPort.BytesToRead}");
            return false;
        }


        public void Dispose()
        {
            try
            {
                serialPort?.Close();
                serialPort?.Dispose();
            }
            catch { }
        }
    }
}
