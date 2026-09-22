using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM03A
{
    public class EventData
    {
        public DateTime CurrentDateTime { get; set; }
        public DateTime EventDateTime { get; set; }
        public byte MeterType { get; set; }
        public string SerialNumber { get; set; }
        public byte EventCode { get; set; }
    }
}
