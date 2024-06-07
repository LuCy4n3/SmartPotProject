using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RJCP.IO.Ports;

namespace Programmer
{
    public static class UtilityExtenssion
    {

        public static void WriteBytes(this SerialPortStream serialPortStream, params UInt16[] dataWords)
        {
            foreach (var word in dataWords)
            {
                serialPortStream.Write(BitConverter.GetBytes(word), 0, 2);
            }
        }
        public static void WaitForDataToBeSentOnWire(this SerialPortStream serialPortStream, int bytesCount)
        {
            double bytesPerMillisecond = serialPortStream.BaudRate;
            bytesPerMillisecond = bytesPerMillisecond / 10 / 1000;//10 bit for one byte, 1000 ms in a second
            double amountToWait = bytesPerMillisecond * bytesCount;
            amountToWait *= 1.025;
            amountToWait += 9;
            Stopwatch sw = Stopwatch.StartNew();
            try{ serialPortStream.Flush();}catch{}
            while (sw.Elapsed.TotalMilliseconds <= amountToWait)//busy wait
            {

            }
        }
        public static int WriteOneByteArrayAndAdditionalBytes(this SerialPortStream serialPortStream, bool flush, byte[] firstSetOfBytes, params Byte[] otherBytes)
        {
            serialPortStream.Write(firstSetOfBytes, 0, firstSetOfBytes.Length);
            serialPortStream.Write(otherBytes, 0, otherBytes.Length);
            var totalBytesCount = firstSetOfBytes.Length + otherBytes.Length;
            if (flush)
            {
                serialPortStream.Flush();
                serialPortStream.WaitForDataToBeSentOnWire(totalBytesCount);
            }

            return totalBytesCount;
        }
        public static async Task<Message> ReadMessageAsync_Old(this SerialPortStream serialPortStream, Int32 timeoutMilliseconds, UInt16 expectedDeviceId, UInt16 expectedMessage, UInt32? expectedData = null)
        {
            int messageLength = 9;
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.Elapsed.TotalMilliseconds < timeoutMilliseconds && serialPortStream.BytesToRead < messageLength)
            {
                await Task.Delay(1);
            }

            if (serialPortStream.BytesToRead > 0)
            {

            }

            if (serialPortStream.BytesToRead < messageLength)
            {
                var msg = new Message
                {
                    Error = $"Timeout in reading message. Available bytes are {serialPortStream.BytesToRead}"
                };
                msg.BytesRead = serialPortStream.BytesToRead;
                if (serialPortStream.BytesToRead > 0)
                {
                    //clean buffer
                    try { serialPortStream.ReadExisting(); } catch { }
                }
                return msg;
            }
            else
            {
                //we have the message
                var msg = new Message { EntireMessage = new byte[messageLength] };
                msg.BytesRead += serialPortStream.Read(msg.EntireMessage, 0, messageLength);
                msg.MagicValue = BitConverter.ToUInt16(msg.EntireMessage);
                if (msg.MagicValue != InputSettings.MAGIC_HEADER)
                {
                    msg.Error =
                        $"Invalid magic header. Got {msg.EntireMessage.Take(2).ToHexString(2)} expected:{InputSettings.MAGIC_HEADER.ToHexString()}";
                    return msg;
                }
                msg.DeviceId = msg.EntireMessage[2];
                msg.MessageId = BitConverter.ToUInt16(msg.EntireMessage, 3);
                msg.Data = BitConverter.ToUInt32(msg.EntireMessage, 5);
                StringBuilder errBuilder = new StringBuilder();
                if (msg.DeviceId != expectedDeviceId)
                {
                    errBuilder.Append(
                        $"Wrong deviceid. {msg.DeviceId.ToHexString()}!={expectedDeviceId.ToHexString()}");
                }
                if (msg.MessageId != expectedMessage)
                {
                    errBuilder.Append(
                        $"Wrong message id received. {msg.MessageId.ToHexString()}!={expectedMessage.ToHexString()}");
                }

                //if (expectedData != null && msg.Data!= expectedData)
                //{
                //    errBuilder.Append(
                //        $"Wrong expectedData. {msg.Data.ToHexString()}!={expectedData.Value.ToHexString()}");
                //}

                msg.Error = errBuilder.ToString();
                return msg;
            }
        }

        public static async Task<Message> ReadMessageAsync(this SerialPortStream serialPortStream, Int32 timeoutMilliseconds, UInt16 expectedDeviceId, UInt16 expectedMessage,
            byte[] ringBuffer = null, UInt32? expectedData = null)
        {
            if (ringBuffer == null)
                ringBuffer = new byte[9];
            int readAmount = 0;
            int messageLength = 9;

            Stopwatch sw = Stopwatch.StartNew();
            bool magicPackageReceived = false;
            while (sw.Elapsed.TotalMilliseconds < timeoutMilliseconds && !magicPackageReceived)
            {
                if (serialPortStream.BytesToRead > 0)
                {
                    byte[] oneByte = new byte[1];
                    serialPortStream.Read(oneByte, 0, oneByte.Length);

                    readAmount++;
                    for (var i = 0; i < ringBuffer.Length - 1; i++)
                    {
                        ringBuffer[i] = ringBuffer[i + 1];
                    }

                    ringBuffer[ringBuffer.Length - 1] = oneByte[0];
                    var magicHeader = BitConverter.ToUInt16(ringBuffer);
                    if (magicHeader == InputSettings.MAGIC_HEADER)
                    {
                        Console.WriteLine("Valid magic header");
                        break;
                    }
                }

                await Task.Delay(2);
            }

            Console.WriteLine($"Read amount:{readAmount}. Ring buffer:{ringBuffer.ToHexString()}");
            //we have the message
            var msg = new Message { EntireMessage = ringBuffer };
            msg.BytesRead += ringBuffer.Length;
            msg.MagicValue = BitConverter.ToUInt16(msg.EntireMessage);
            if (msg.MagicValue != InputSettings.MAGIC_HEADER)
            {
                msg.Error =
                    $"Invalid magic header. Got {msg.EntireMessage.Take(2).ToHexString(2)} expected:{InputSettings.MAGIC_HEADER.ToHexString()}";
                return msg;
            }

            msg.DeviceId = msg.EntireMessage[2];
            msg.MessageId = BitConverter.ToUInt16(msg.EntireMessage, 3);
            msg.Data = BitConverter.ToUInt32(msg.EntireMessage, 5);
            StringBuilder errBuilder = new StringBuilder();
            if (msg.DeviceId != expectedDeviceId)
            {
                errBuilder.Append(
                    $"Wrong deviceid. {msg.DeviceId.ToHexString()}!={expectedDeviceId.ToHexString()}");
            }

            if (msg.MessageId != expectedMessage)
            {
                errBuilder.Append(
                    $"Wrong message id received. {msg.MessageId.ToHexString()}!={expectedMessage.ToHexString()}");
            }

            //if (expectedData != null && msg.Data!= expectedData)
            //{
            //    errBuilder.Append(
            //        $"Wrong expectedData. {msg.Data.ToHexString()}!={expectedData.Value.ToHexString()}");
            //}

            msg.Error = errBuilder.ToString();
            return msg;
        }

        public static string ToHexString(this UInt16 val)
        {
            return ToHexString(BitConverter.GetBytes(val), 2);
        }
        public static string ToHexString(this UInt32 val)
        {
            return ToHexString(BitConverter.GetBytes(val), 4);
        }
        public static string ToHexString(this byte[] Bytes) => "0x" + BitConverter.ToString(Bytes, 0, Bytes.Length).Replace("-", "");
        public static string ToHexString(this IEnumerable<byte> Bytes, int length) => ToHexString(Bytes.Take(length).ToArray());
    }
}
