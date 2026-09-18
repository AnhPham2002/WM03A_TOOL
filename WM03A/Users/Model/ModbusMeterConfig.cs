using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM03A
{
    public class ModbusMeterConfig
    {
        public bool MeterEnable { get; set; }
        public string SerialNumber { get; set; }

        public byte SlaveAddress { get; set; }
        public uint BaudRate { get; set; }
        public byte SerialConfig { get; set; }
        public byte ReadFuncCode { get; set; }

        public ModbusParameterConfig ForwardTotal { get; set; }
        public ModbusParameterConfig ReverseTotal { get; set; }
        public ModbusParameterConfig FlowRate { get; set; }
    }

    public class ModbusParameterConfig
    {
        public bool ParameterEnable { get; set; }
        public ushort RegisterAddress { get; set; }
        public byte DataType { get; set; }
        public byte WordSwap { get; set; }
        public sbyte Multiplier { get; set; }
    }
}
