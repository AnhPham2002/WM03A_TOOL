using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM03A
{
    public class PulseMeterConfig
    {
        public bool MeterEnable { get; set; }
        public string SerialNumber { get; set; }
        public ushort PulseFactor { get; set; }
        public byte PulseType { get; set; }
        public byte Pin1 { get; set; }
        public byte Pin2 { get; set; }
        public byte EdgeType { get; set; }
    }
}
