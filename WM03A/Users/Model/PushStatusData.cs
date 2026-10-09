using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM03A
{
    public class PushStatusData
    {
        public DateTime CurrentDateTime { get; set; }
        public DateTime PushDateTime { get; set; }
        public ushort SessionDuration { get; set; }
        public sbyte Rssi { get; set; }
        public sbyte Rsrp { get; set; }
        public sbyte Rsrq { get; set; }
        public sbyte Rssnr { get; set; }
        public byte CellularError { get; set; }
        public byte PushError { get; set; }
    }
}
