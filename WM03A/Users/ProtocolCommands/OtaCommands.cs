using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WM03A.Protocol;

namespace WM03A
{
    internal class OtaCommands
    {
        private const int VERSION_SIZE = 10;
        public static bool Request(string firmwareVersion, uint sizeA, uint crcA, uint sizeB, uint crcB, out byte[] frame)
        {
            frame = null;

            byte[] payload = new byte[VERSION_SIZE + sizeof(uint) + sizeof(uint) + sizeof(uint) + sizeof(uint)];

            int index = 0;

            byte[] version = Encoding.ASCII.GetBytes(firmwareVersion);

            if (version.Length > VERSION_SIZE)
            {
                return false;
            }

            Array.Copy(version, 0, payload, index, version.Length);
            index += VERSION_SIZE;

            Array.Copy(BitConverter.GetBytes(sizeA), 0, payload, index, sizeof(uint));
            index += sizeof(uint);

            Array.Copy(BitConverter.GetBytes(crcA), 0, payload, index, sizeof(uint));
            index += sizeof(uint);

            Array.Copy(BitConverter.GetBytes(sizeB), 0, payload, index, sizeof(uint));
            index += sizeof(uint);

            Array.Copy(BitConverter.GetBytes(crcB), 0, payload, index, sizeof(uint));

            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Ota,
                id: (byte)OtaId.UpdateRequest,
                payload: payload,
                out frame);
        }

        public static bool SendPacket(ushort packetIndex, byte[] packet, out byte[] frame)
        {
            frame = null;

            if (packet == null || packet.Length > ushort.MaxValue)
            {
                return false;
            }

            ushort packetSize = (ushort)packet.Length;

            byte[] payload = new byte[sizeof(ushort) + sizeof(ushort) + packet.Length];

            int index = 0;

            Array.Copy(BitConverter.GetBytes(packetIndex), 0, payload, index, sizeof(ushort));
            index += sizeof(ushort);

            Array.Copy(BitConverter.GetBytes(packetSize), 0, payload, index, sizeof(ushort));
            index += sizeof(ushort);

            Array.Copy(packet, 0, payload, index, packet.Length);

            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Ota,
                id: (byte)OtaId.SendPacket,
                payload: payload,
                out frame);
        }
    }
}
