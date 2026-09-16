using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WM03A.Protocol;

namespace WM03A
{
    internal class QueryParser
    {
        public struct MeterData
        {
            public byte[] MeterSerial;
            public double ForwardTotalizer;
            public double ReverseTotalizer;
            public double FlowRate;
        }

        public static bool SimNetworkInfo(byte[] frame, out byte[] ccid, out sbyte rssi, out sbyte rsrp, out sbyte rsrq, out sbyte rssnr)
        {
            ccid = null;
            rssi = default;
            rsrp = default;
            rsrq = default;
            rssnr = default;
            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);
            if (payload.Length < 29)
                return false;
            ccid = payload.Skip(0).Take(25).ToArray();
            rssi = unchecked((sbyte)payload[25]);
            rsrp = unchecked((sbyte)payload[26]);
            rsrq = unchecked((sbyte)payload[27]);
            rssnr = unchecked((sbyte)payload[28]);
            return true;
        }

        public static bool PulseMeterData(byte[] frame, out MeterData data)
        {
            data = default;
            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);
            if (payload.Length < 44)
                return false;
            data.MeterSerial = payload.Skip(0).Take(20).ToArray();
            data.ForwardTotalizer = BitConverter.ToDouble(payload, 20);
            data.ReverseTotalizer = BitConverter.ToDouble(payload, 28);
            data.FlowRate = BitConverter.ToDouble(payload, 36);
            return true;
        }

        public static bool ModbusMeterData(byte[] frame, out MeterData data)
        {
            data = default;
            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);
            if (payload.Length < 44)
                return false;
            data.MeterSerial = payload.Skip(0).Take(20).ToArray();
            data.ForwardTotalizer = BitConverter.ToDouble(payload, 20);
            data.ReverseTotalizer = BitConverter.ToDouble(payload, 28);
            data.FlowRate = BitConverter.ToDouble(payload, 36);
            return true;
        }

        public static bool PressureSensorData(byte[] frame, out byte[] meterSerial, out float data)
        {
            meterSerial = default;
            data = default;
            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);
            if (payload.Length < 24)
                return false;
            meterSerial = payload.Skip(0).Take(20).ToArray();
            data = BitConverter.ToSingle(payload, 20);
            return true;
        }
    }
}
