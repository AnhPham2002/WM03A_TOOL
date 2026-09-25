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
        public static bool ModuleSerial(byte[] frame)
        {
            ProtocolErrCode errCodeUnpack = UnpackAck(frame, out DateTime dateTime, out byte cmd, out byte id, out ProtocolErrCode errCode);
            if ((errCodeUnpack != ProtocolErrCode.Success) || (errCode != ProtocolErrCode.Success) || (cmd != (byte)CmdCode.Set) || (id != (byte)ConfigId.ModuleSerial))
                return false;
            return true;
        }

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

        public static bool PressureSensor(byte[] frame)
        {
            ProtocolErrCode errCodeUnpack = UnpackAck(frame, out DateTime dateTime, out byte cmd, out byte id, out ProtocolErrCode errCode);
            if ((errCodeUnpack != ProtocolErrCode.Success) || (errCode != ProtocolErrCode.Success) || (cmd != (byte)CmdCode.Set) || (id != (byte)ConfigId.PressureSensor))
                return false;
            return true;
        }

        public static bool ResetSetting(byte[] frame)
        {
            ProtocolErrCode errCodeUnpack = UnpackAck(frame, out DateTime dateTime, out byte cmd, out byte id, out ProtocolErrCode errCode);
            if ((errCodeUnpack != ProtocolErrCode.Success) || (errCode != ProtocolErrCode.Success) || (cmd != (byte)CmdCode.Set) || (id != (byte)ConfigId.ResetSetting))
                return false;
            return true;
        }

        public static bool ChangePassword(byte[] frame)
        {
            ProtocolErrCode errCodeUnpack = UnpackAck(frame, out DateTime dateTime, out byte cmd, out byte id, out ProtocolErrCode errCode);
            if ((errCodeUnpack != ProtocolErrCode.Success) || (errCode != ProtocolErrCode.Success) || (cmd != (byte)CmdCode.Set) || (id != (byte)ConfigId.ChangePassword))
                return false;
            return true;
        }

        public static bool ResetPassword(byte[] frame)
        {
            ProtocolErrCode errCodeUnpack = UnpackAck(frame, out DateTime dateTime, out byte cmd, out byte id, out ProtocolErrCode errCode);
            if ((errCodeUnpack != ProtocolErrCode.Success) || (errCode != ProtocolErrCode.Success) || (cmd != (byte)CmdCode.Set) || (id != (byte)ConfigId.ResetPassword))
                return false;
            return true;
        }

        public static bool McuResetCount(byte[] frame)
        {
            ProtocolErrCode errCodeUnpack = UnpackAck(frame, out DateTime dateTime, out byte cmd, out byte id, out ProtocolErrCode errCode);
            if ((errCodeUnpack != ProtocolErrCode.Success) || (errCode != ProtocolErrCode.Success) || (cmd != (byte)CmdCode.Set) || (id != (byte)ConfigId.McuResetCount))
                return false;
            return true;
        }

        public static bool EraseLatchData(byte[] frame)
        {
            ProtocolErrCode errCodeUnpack = UnpackAck(frame, out DateTime dateTime, out byte cmd, out byte id, out ProtocolErrCode errCode);
            if ((errCodeUnpack != ProtocolErrCode.Success) || (errCode != ProtocolErrCode.Success) || (cmd != (byte)CmdCode.Set) || (id != (byte)ConfigId.EraseMeasurementData))
                return false;
            return true;
        }

        public static bool EraseEventData(byte[] frame)
        {
            ProtocolErrCode errCodeUnpack = UnpackAck(frame, out DateTime dateTime, out byte cmd, out byte id, out ProtocolErrCode errCode);
            if ((errCodeUnpack != ProtocolErrCode.Success) || (errCode != ProtocolErrCode.Success) || (cmd != (byte)CmdCode.Set) || (id != (byte)ConfigId.EraseEventData))
                return false;
            return true;
        }

        public static bool EraseErrLog(byte[] frame)
        {
            ProtocolErrCode errCodeUnpack = UnpackAck(frame, out DateTime dateTime, out byte cmd, out byte id, out ProtocolErrCode errCode);
            if ((errCodeUnpack != ProtocolErrCode.Success) || (errCode != ProtocolErrCode.Success) || (cmd != (byte)CmdCode.Set) || (id != (byte)ConfigId.EraseLogData))
                return false;
            return true;
        }

        public static bool LatchActive(byte[] frame)
        {
            ProtocolErrCode errCodeUnpack = UnpackAck(frame, out DateTime dateTime, out byte cmd, out byte id, out ProtocolErrCode errCode);
            if ((errCodeUnpack != ProtocolErrCode.Success) || (errCode != ProtocolErrCode.Success) || (cmd != (byte)CmdCode.Set) || (id != (byte)ConfigId.LatchImmediately))
                return false;
            return true;
        }
        public static bool EventCreate(byte[] frame)
        {
            ProtocolErrCode errCodeUnpack = UnpackAck(frame, out DateTime dateTime, out byte cmd, out byte id, out ProtocolErrCode errCode);
            if ((errCodeUnpack != ProtocolErrCode.Success) || (errCode != ProtocolErrCode.Success) || (cmd != (byte)CmdCode.Set) || (id != (byte)ConfigId.EventCreate))
                return false;
            return true;
        }

        public static bool PushActive(byte[] frame)
        {
            ProtocolErrCode errCodeUnpack = UnpackAck(frame, out DateTime dateTime, out byte cmd, out byte id, out ProtocolErrCode errCode);
            if ((errCodeUnpack != ProtocolErrCode.Success) || (errCode != ProtocolErrCode.Success) || (cmd != (byte)CmdCode.Set) || (id != (byte)ConfigId.PushImmediately))
                return false;
            return true;
        }

        public static bool FactoryReset(byte[] frame)
        {
            ProtocolErrCode errCodeUnpack = UnpackAck(frame, out DateTime dateTime, out byte cmd, out byte id, out ProtocolErrCode errCode);
            if ((errCodeUnpack != ProtocolErrCode.Success) || (errCode != ProtocolErrCode.Success) || (cmd != (byte)CmdCode.Set) || (id != (byte)ConfigId.FactoryReset))
                return false;
            return true;
        }
    }
}
