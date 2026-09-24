using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WM03A.Protocol;

namespace WM03A
{
    internal class OtaParser
    {
        public static bool Request(byte[] frame, out byte otaSlot,out ushort packetIndex)
        {
            otaSlot = default;
            packetIndex = default;
            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);
            if (payload.Length < 3)
                return false;
            otaSlot = payload[0];
            packetIndex = BitConverter.ToUInt16(payload, 1);
            return true;
        }

        public static bool SendPacket(byte[] frame)
        {
            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);
            if (payload.Length < 3)
                return false;
            if (payload[2] != (byte)ProtocolErrCode.Success)
                return false;
            return true;
        }
    }
}
