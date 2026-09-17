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
    }
}
