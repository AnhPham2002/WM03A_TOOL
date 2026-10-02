using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM03A
{
    public class ChargeConfig
    {
        public ushort IinLimMa { get; set; }
        public ushort ChargeVoltageMv { get; set; }
        public ushort ChargeCurrentMa { get; set; }
        public bool ChargeLedEnable { get; set; }
    }
}
