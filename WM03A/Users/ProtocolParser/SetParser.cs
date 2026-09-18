using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WM03A.Protocol;

namespace WM03A.Users.ProtocolParser
{
    internal class SetParser
    {
        public static bool DateTime(byte[] frame)
        {
            ProtocolErrCode errCodeUnpack = UnpackAck(frame, out DateTime dateTime, out byte cmd, out byte id, out ProtocolErrCode errCode);
            if ((errCodeUnpack != ProtocolErrCode.Success) || (errCode != ProtocolErrCode.Success) || (cmd != (byte)CmdCode.Set) || (id != (byte)ConfigId.Time))
                return false;
            return true;
        }

        public static bool IpEndpoint(byte[] frame)
        {
            ProtocolErrCode errCodeUnpack = UnpackAck(frame, out DateTime dateTime, out byte cmd, out byte id, out ProtocolErrCode errCode);
            if ((errCodeUnpack != ProtocolErrCode.Success) || (errCode != ProtocolErrCode.Success) || (cmd != (byte)CmdCode.Set) || (id != (byte)ConfigId.IpEndpoint))
                return false;
            return true;
        }

        public static bool ModuleConfig(byte[] frame)
        {
            ProtocolErrCode errCodeUnpack = UnpackAck(frame, out DateTime dateTime, out byte cmd, out byte id, out ProtocolErrCode errCode);
            if ((errCodeUnpack != ProtocolErrCode.Success) || (errCode != ProtocolErrCode.Success) || (cmd != (byte)CmdCode.Set) || (id != (byte)ConfigId.Module))
                return false;
            return true;
        }

        public static bool PulseMeter(byte[] frame)
        {
            ProtocolErrCode errCodeUnpack = UnpackAck(frame, out DateTime dateTime, out byte cmd, out byte id, out ProtocolErrCode errCode);
            if ((errCodeUnpack != ProtocolErrCode.Success) || (errCode != ProtocolErrCode.Success) || (cmd != (byte)CmdCode.Set) || (id != (byte)ConfigId.PulseMeter))
                return false;
            return true;
        }

        public static bool ModbusMeter(byte[] frame)
        {
            ProtocolErrCode errCodeUnpack = UnpackAck(frame, out DateTime dateTime, out byte cmd, out byte id, out ProtocolErrCode errCode);
            if ((errCodeUnpack != ProtocolErrCode.Success) || (errCode != ProtocolErrCode.Success) || (cmd != (byte)CmdCode.Set) || (id != (byte)ConfigId.ModbusMeter))
                return false;
            return true;
        }
    }
}
