using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM03A
{
    public class FirmwareMetadata
    {
        public uint Sequence { get; set; }
        public string Version { get; set; }
        public uint Size { get; set; }
        public uint Crc { get; set; }
    }
}
