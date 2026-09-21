using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM03A.Users.Model
{
    public class PressureSensorConfig
    {
        public bool SensorEnable { get; set; }
        public string SerialNumber { get; set; }
        public float MinCurrent { get; set; }
        public float MaxCurrent { get; set; }
        public float MinPressure { get; set; }
        public float MaxPressure { get; set; }
    }
}
