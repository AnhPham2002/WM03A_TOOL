using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM03A.Users.Model
{
    public enum MeterType : byte
    {
        ModuleType = 0,
        PulseMeterType,
        ModbusMeterType,
        PressureSensorType
    }

    public class LatchData
    {
        public DateTime CurrentDateTime { get; set; }
        public DateTime LatchDateTime { get; set; }
        public List<LatchMeterData> Meters { get; set; } = new List<LatchMeterData>();
    }

    public class LatchMeterData
    {
        public byte MeterType { get; set; }
        public string SerialNumber { get; set; }
        public double ForwardTotalizer { get; set; }
        public double ReverseTotalizer { get; set; }
        public double FlowRate { get; set; }
        public float Pressure { get; set; }
    }
}
