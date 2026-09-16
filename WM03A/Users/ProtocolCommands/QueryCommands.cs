using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WM03A.Protocol;

namespace WM03A
{
    internal class QueryCommands
    {
        public static bool SimNetworkInfo(out byte[] frame)
        {
            return Pack(
                encrypt: false,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Query,
                id: (byte)QueryId.SimNetworkInfo,
                payload: null,
                out frame);
        }

        public static bool PulseMeterData(ushort meterIndex, out byte[] frame)
        {
            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Query,
                id: (byte)QueryId.PulseMeterData,
                payload: new byte[] { (byte)meterIndex },
                out frame);
        }

        public static bool ModbusMeterData(ushort meterIndex, out byte[] frame)
        {
            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Query,
                id: (byte)QueryId.ModbusMeterData,
                payload: new byte[] { (byte)meterIndex },
                out frame);
        }

        public static bool PressureSensorData(ushort meterIndex, out byte[] frame)
        {
            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Query,
                id: (byte)QueryId.PressureSensorData,
                payload: new byte[] { (byte)meterIndex },
                out frame);
        }
    }
}
