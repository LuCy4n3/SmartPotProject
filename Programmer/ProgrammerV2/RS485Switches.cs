using RJCP.IO.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using RS485Master;

namespace ProgrammerV2
{
    internal class RS485Switches
    {
        private static SerialPortStream serialPort;
        public static async Task PerformInitialSetupAsync()
        {
            string comPort = "COM3";
            serialPort = new SerialPortStream(comPort, 9600);
            serialPort.Open();

            while (true)
            {
                Console.Write("Address in decimal:");
                byte deviceNewAddress = byte.Parse(Console.ReadLine().Trim());
                Console.WriteLine();
                serialPort.RtsEnable = serialPort.DtrEnable = false; //set MAX485 transceiver in send mode
                await Task.Delay(100);

                List<byte> bytesToWrite = FromHexString("FF 06 10 00 00");
                bytesToWrite.Add(deviceNewAddress);
                Console.WriteLine($"Setting address to:{Convert.ToHexString(new[] { deviceNewAddress })}");
                await SendDataAndCrcAsync(bytesToWrite);
                await Task.Delay(100);
                
                await SendDataAndCrcAsync(FromHexString("02 06 10 0F 00 FE 3C BA"),false);
                await Task.Delay(100);
                
                Console.WriteLine("Sending new config to device");
                bytesToWrite = FromHexString("01 06 10 03 00");
                bytesToWrite[0] = deviceNewAddress;
                bytesToWrite.Add(0b00110110);//config
                await SendDataAndCrcAsync(bytesToWrite);
                await Task.Delay(100);

                Console.WriteLine("Sending lights on cmd. Press enter to continue");
                Console.ReadLine();

                bytesToWrite = FromHexString("02 06 10 08 01 01");
                bytesToWrite[0] = deviceNewAddress;
                await SendDataAndCrcAsync(bytesToWrite);
                await Task.Delay(100);

                Console.WriteLine("Sending back lights off cmd. Press enter to continue");
                Console.ReadLine();
                bytesToWrite = FromHexString("02 06 10 08 00 03");
                bytesToWrite[0] = deviceNewAddress;
                await SendDataAndCrcAsync(bytesToWrite);
                await Task.Delay(100);

                serialPort.RtsEnable = serialPort.DtrEnable = true; //set MAX485 in receive mode
                Console.WriteLine("Listening for data from switch");
                while (!Console.KeyAvailable)
                {
                    //break;
                    await Task.Delay(100); //wait for any data
                    if (serialPort.BytesToRead > 0)
                    {
                        await Task.Delay(500); //wait for any data
                        byte[] barr = new byte[serialPort.BytesToRead]; //serialPort.BytesToRead will give available bytes to read from kernel buffer
                        serialPort.Read(barr, 0, barr.Length);
                        if (barr.Length > 0)
                            Console.WriteLine($"Data length:{barr.Length}. Hex:{ToHexString(barr)}"); //no bytes received and the switch LED timeout is still 1000 ms (default value)
                    }
                }

                Console.ReadLine();
            }
        }

        private static async Task SendDataAndCrcAsync(IEnumerable<byte> bytes, bool sendCrc = true)
        {
            var dataBuff = bytes.ToArray();

            serialPort.Write(dataBuff, 0, dataBuff.Length);
            int bytesCount = dataBuff.Length;
            if (sendCrc)
            {
                var crc = ModRTU_CRC(dataBuff); //compute Modbus CRC16 using 0xA001 polynomial and 0xFFFF starting crc
                serialPort.Write(BitConverter.GetBytes(crc).ToArray(), 0, 2); //sending the 2 bytes of CRC. I tried reverse the CRC bytes and also normal order
                bytesCount += 2;
            }

            serialPort.WaitForDataToBeSentOnWire(bytesCount); //waiting for MAX485 transceiver to send the data on the wire
        }

        private static List<byte> FromHexString(string hexString)
        {
            return hexString.Split(' ').Select(c => Convert.ToByte($"{c}", 16)).ToList();
        }
        static UInt16 ModRTU_CRC(IEnumerable<byte> buf)
        {
            UInt16 crc = 0xFFFF;

            foreach (var oneByte in buf)
            {
                crc ^= (UInt16)oneByte;          // XOR byte into least sig. byte of crc

                for (int i = 8; i != 0; i--)
                {    // Loop over each bit
                    if ((crc & 0x0001) != 0)
                    {      // If the LSB is set
                        crc >>= 1;                    // Shift right and XOR 0xA001
                        crc ^= 0xA001;
                        //crc ^= 0x8005;
                    }
                    else                            // Else LSB is not set
                        crc >>= 1;                    // Just shift right
                }
            }
            // Note, this number has low and high bytes swapped, so use it accordingly (or swap bytes)
            return crc;
        }
        static string ToHexString(byte[] Bytes) => "0x" + BitConverter.ToString(Bytes, 0, Bytes.Length).Replace("-", "");
        static string ToHexString(IEnumerable<byte> Bytes, int length) => ToHexString(Bytes.Take(length).ToArray());

    }
}
