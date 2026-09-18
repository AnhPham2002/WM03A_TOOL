using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WM03A.Protocol;

namespace WM03A.Users.ProtocolCommands
{
    internal class SetCommands
    {
        public static bool DateTime(DateTime dateTime, out byte[] frame)
        {
            byte[] payload =
            {
                (byte)(dateTime.Year % 100),
                (byte)dateTime.Month,
                (byte)dateTime.Day,
                (byte)dateTime.Hour,
                (byte)dateTime.Minute,
                (byte)dateTime.Second
            };
            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Set,
                id: (byte)ConfigId.Time,
                payload: payload,
                out frame);
        }

        public static bool IpEndpoint(string ip, string port, out byte[] frame)
        {
            frame = null;

            if (string.IsNullOrWhiteSpace(ip) && string.IsNullOrWhiteSpace(port))
            {
                return false;
            }

            byte[] ipBytes = Encoding.ASCII.GetBytes(ip.Trim());
            byte[] portBytes = Encoding.ASCII.GetBytes(port.Trim());

            if (ipBytes.Length > 15 || portBytes.Length > 5)
            {
                return false;
            }

            byte[] payload = new byte[20];

            Buffer.BlockCopy(ipBytes, 0, payload, 0, ipBytes.Length);
            Buffer.BlockCopy(portBytes, 0, payload, 15, portBytes.Length);

            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Set,
                id: (byte)ConfigId.IpEndpoint,
                payload: payload,
                out frame);
        }

        public static bool ModuleConfig(string latchPeriod, string pushPeriod, string timezone, out byte[] frame)
        {
            frame = null;

            List<byte> payload = new List<byte>();

            if (string.IsNullOrWhiteSpace(latchPeriod) && string.IsNullOrWhiteSpace(pushPeriod) && string.IsNullOrWhiteSpace(timezone))
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(latchPeriod))
            {
                ushort value = ushort.Parse(latchPeriod);

                payload.Add((byte)ConfigModuleId.LatchPeriod);
                payload.AddRange(BitConverter.GetBytes(value));
            }

            if (!string.IsNullOrWhiteSpace(pushPeriod))
            {
                ushort value = ushort.Parse(pushPeriod);

                payload.Add((byte)ConfigModuleId.PushPeriod);
                payload.AddRange(BitConverter.GetBytes(value));
            }

            if (!string.IsNullOrWhiteSpace(timezone))
            {
                float value = float.Parse(timezone);

                payload.Add((byte)ConfigModuleId.Timezone);
                payload.AddRange(BitConverter.GetBytes(value));
            }

            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Set,
                id: (byte)ConfigId.Module,
                payload: payload.ToArray(),
                out frame);
        }

        public static bool PulseMeter(byte meterIndex, bool? enabled, string serialNumber, ushort? pulseFactor, byte? pulseType, byte? pin1, byte? pin2, byte? edgeType, double? forward, double? reverse, out byte[] frame)
        {
            frame = null;

            List<byte> payload = new List<byte>();
            payload.Add(meterIndex);

            if (enabled.HasValue && !enabled.Value)
            {
                payload.Add((byte)ConfigPulseMeterId.MeterEnable);
                payload.Add(0x00);

                payload.Add((byte)ConfigPulseMeterId.SerialNumber);
                for (int i = 0; i < METER_SERIAL_SIZE; i++)
                {
                    payload.Add(0x00);
                }

                payload.Add((byte)ConfigPulseMeterId.PulseFactor);
                payload.Add(0x00);
                payload.Add(0x00);

                payload.Add((byte)ConfigPulseMeterId.PulseType);
                payload.Add(0x00);

                payload.Add((byte)ConfigPulseMeterId.Pin1);
                payload.Add(0x00);

                payload.Add((byte)ConfigPulseMeterId.Pin2);
                payload.Add(0x00);

                payload.Add((byte)ConfigPulseMeterId.EdgeType);
                payload.Add(0x00);

                payload.Add((byte)ConfigPulseMeterId.MeterData);
                payload.AddRange(new byte[sizeof(double)]);
                payload.AddRange(new byte[sizeof(double)]);
            }
            else
            {
                if (enabled.HasValue)
                {
                    payload.Add((byte)ConfigPulseMeterId.MeterEnable);
                    payload.Add(enabled.Value ? (byte)1 : (byte)0);
                }

                if (!string.IsNullOrWhiteSpace(serialNumber))
                {
                    byte[] serialBytes = Encoding.ASCII.GetBytes(serialNumber.Trim());

                    if (serialBytes.Length > METER_SERIAL_SIZE)
                    {
                        return false;
                    }

                    payload.Add((byte)ConfigPulseMeterId.SerialNumber);
                    payload.AddRange(serialBytes);

                    for (int i = serialBytes.Length; i < METER_SERIAL_SIZE; i++)
                    {
                        payload.Add(0x00);
                    }
                }

                if (pulseFactor.HasValue)
                {
                    payload.Add((byte)ConfigPulseMeterId.PulseFactor);
                    payload.AddRange(BitConverter.GetBytes(pulseFactor.Value));
                }

                if (pulseType.HasValue)
                {
                    payload.Add((byte)ConfigPulseMeterId.PulseType);
                    payload.Add(pulseType.Value);
                }

                if (pin1.HasValue)
                {
                    payload.Add((byte)ConfigPulseMeterId.Pin1);
                    payload.Add(pin1.Value);
                }

                if (pin2.HasValue)
                {
                    payload.Add((byte)ConfigPulseMeterId.Pin2);
                    payload.Add(pin2.Value);
                }

                if (edgeType.HasValue)
                {
                    payload.Add((byte)ConfigPulseMeterId.EdgeType);
                    payload.Add(edgeType.Value);
                }

                if (forward.HasValue != reverse.HasValue)
                {
                    return false;
                }

                if (forward.HasValue && reverse.HasValue)
                {
                    payload.Add((byte)ConfigPulseMeterId.MeterData);
                    payload.AddRange(BitConverter.GetBytes(forward.Value));
                    payload.AddRange(BitConverter.GetBytes(reverse.Value));
                }
            }

            if (payload.Count <= 1)
            {
                return false;
            }

            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Set,
                id: (byte)ConfigId.PulseMeter,
                payload: payload.ToArray(),
                out frame);
        }
    }
}
