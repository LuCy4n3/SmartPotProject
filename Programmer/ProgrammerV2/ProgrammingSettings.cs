using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammerV2
{
    internal class ProgrammingSettings
    {
        public byte DeviceId{ get; init; }
        public string SerialPort { get; init; }
        public string FilePath { get; init; }
        public string GpioPin { get; init; }
        public int BaudRate{ get; init; } 
    }
}
