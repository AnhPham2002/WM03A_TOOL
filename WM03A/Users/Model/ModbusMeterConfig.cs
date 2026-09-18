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
        public bool WordSwap { get; set; }
        public sbyte Multiplier { get; set; }
    }

    public enum ModbusSerialConfig : byte
    {
        Modbus_8N1 = 0,
        Modbus_8O1,
        Modbus_8E1,
    }

    public enum ModbusDataType : byte
    {
        ModbusInt16 = 1,
        ModbusUint16,
        ModbusInt32,
        ModbusUint32,
        ModbusFloat,
        ModbusInt64,
        ModbusUInt64,
        ModbusDouble
    }
}
