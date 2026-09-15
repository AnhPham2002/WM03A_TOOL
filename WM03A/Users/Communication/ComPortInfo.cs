using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM03A
{
    public class ComPortInfo
    {
        public string PortName { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{PortName} ({Description})";
        }
    }
}
