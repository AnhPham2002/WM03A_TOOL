using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WM03A.Protocol;

namespace WM03A
{
    internal class GetCommands
    {
        public static bool ModuleSerial(out byte[] frame)
        {
            return Pack(
                encrypt: false,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Get,
                id: (byte)ConfigId.ModuleSerial,
                payload: null,
                out frame);
        }

        public static bool ModuleActivationStatus(out byte[] frame)
        {
            return Pack(
                encrypt: false,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Get,
                id: (byte)ConfigId.ModuleActivationStatus,
                payload: null,
                out frame);
        }

        public static bool DateTime(out byte[] frame)
        {
            return Pack(
                encrypt: false,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Get,
                id: (byte)ConfigId.Time,
                payload: null,
                out frame);
        }

        public static bool IpEndpoint(out byte[] frame)
        {
            return Pack(
                encrypt: false,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Get,
                id: (byte)ConfigId.IpEndpoint,
                payload: null,
                out frame);
        }

        public static bool LatchPeriod(out byte[] frame)
        {
            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Get,
                id: (byte)ConfigId.Module,
                payload: new byte[] { (byte)(ConfigModuleId.LatchPeriod) },
                out frame);
        }

        public static bool PushPeriod(out byte[] frame)
        {
            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Get,
                id: (byte)ConfigId.Module,
                payload: new byte[] { (byte)(ConfigModuleId.PushPeriod) },
                out frame);
        }

        public static bool Timezone(out byte[] frame)
        {
            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Get,
                id: (byte)ConfigId.Module,
                payload: new byte[] { (byte)(ConfigModuleId.Timezone) },
                out frame);
        }

        public static bool PulseMeter(byte meterIndex, byte[] parameterIds, out byte[] frame)
        {
            frame = null;

            if (parameterIds == null || parameterIds.Length == 0)
            {
                return false;
            }

            byte[] payload = new byte[1 + parameterIds.Length];

            payload[0] = meterIndex;
            Buffer.BlockCopy(parameterIds, 0, payload, 1, parameterIds.Length);

            return Protocol.Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Get,
                id: (byte)ConfigId.PulseMeter,
                payload: payload,
                out frame);
        }

        public static bool ModbusMeter(byte meterIndex, byte[] parameterIds, out byte[] frame)
        {
            frame = null;

            if (parameterIds == null || parameterIds.Length == 0)
            {
                return false;
            }

            byte[] payload = new byte[1 + parameterIds.Length];

            payload[0] = meterIndex;
            Buffer.BlockCopy(parameterIds, 0, payload, 1, parameterIds.Length);

            return Protocol.Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Get,
                id: (byte)ConfigId.ModbusMeter,
                payload: payload,
                out frame);
        }

        public static bool PressureSensor(byte meterIndex, byte[] parameterIds, out byte[] frame)
        {
            frame = null;

            if (parameterIds == null || parameterIds.Length == 0)
            {
                return false;
            }

            byte[] payload = new byte[1 + parameterIds.Length];

            payload[0] = meterIndex;
            Buffer.BlockCopy(parameterIds, 0, payload, 1, parameterIds.Length);

            return Protocol.Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Get,
                id: (byte)ConfigId.PressureSensor,
                payload: payload,
                out frame);
        }

        public static bool McuResetCount(out byte[] frame)
        {
            return Protocol.Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Get,
                id: (byte)ConfigId.McuResetCount,
                payload: null,
                out frame);
        }
    }
}
