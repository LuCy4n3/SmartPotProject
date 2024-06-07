using System;

namespace Programmer
{
    public class Message
    {
        public Int32 BytesRead { get; set; }
        public Byte[] EntireMessage { get; set; }
        public UInt16 MagicValue { get; set; }
        public UInt16 DeviceId { get; set; }

        public UInt16 MessageId { get; set; }

        public UInt32 Data { get; set; }

        public String Error { get; set; }

        public Boolean HasErrors => !String.IsNullOrWhiteSpace(Error);
    }
}
