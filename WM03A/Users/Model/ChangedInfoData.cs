using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM03A
{
    public class ChangedInfoData
    {
        public DateTime CurrentDateTime { get; set; }
        public DateTime ChangedDateTime { get; set; }
        public byte Source { get; set; }
        public byte Level { get; set; }
        public byte ConfigId { get; set; }
        public ulong NewValue { get; set; }
    }
}
