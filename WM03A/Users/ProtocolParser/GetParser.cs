using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WM03A.Protocol;

namespace WM03A
{
    internal class GetParser
    {
        public static ulong ModuleSerial(byte[] frame)
        {
            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);
            return serial;
        }

        //public static bool ModuleActivationStatus(byte[] frame)
        //{
        //    ulong serial;
        //    byte cmd;
        //    byte id;
        //    byte[] payload;
        //    Protocol.Unpack(frame, out serial, out cmd, out id, out payload);
        //    if (payload.Length < 1)
        //        throw new ArgumentException("Payload length is less than 1 byte.");
        //    return payload[0] != 0;
        //}

        public static bool DateTime(byte[] frame, out DateTime time)
        {
            time = default;
            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);
            if (payload.Length < 6)
                return false;
            int year = 2000 + payload[0]; // Assuming year is stored as offset from 2000
            int month = payload[1];
            int day = payload[2];
            int hour = payload[3];
            int minute = payload[4];
            int second = payload[5];
            time = new DateTime(year, month, day, hour, minute, second);
            return true;
        }

        public static bool IpEndpoint(byte[] frame, out string ip, out string port)
        {
            ip = null;
            port = null;
            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);
            if (payload.Length < 20)
                return false;
            ip = Encoding.ASCII.GetString(payload, 0, 15).TrimEnd('\0');
            port = Encoding.ASCII.GetString(payload, 15, 5);
            return true;
        }

        public static ushort LatchPeriod(byte[] frame)
        {
            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);
            if (payload.Length < 3)
                return 0;
            return BitConverter.ToUInt16(payload, 1);
        }

        public static ushort PushPeriod(byte[] frame)
        {
            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);
            if (payload.Length < 3)
                return 0;
            return BitConverter.ToUInt16(payload, 1);
        }

        public static float Timezone(byte[] frame)
        {
            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);
            if (payload.Length < 5)
                return 0.0f;
            return BitConverter.ToSingle(payload, 1);
        }
    }
}
