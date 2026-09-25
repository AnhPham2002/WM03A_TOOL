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

        private const int PASSWORD_LENGTH = 8;
        public static bool ModuleSerial(string serial, out byte[] frame)
        {
            frame = null;

            if (serial.Length != 12 || !serial.All(char.IsDigit))
            {
                return false;
            }

            ulong value = ulong.Parse(serial);
            byte[] payload = BitConverter.GetBytes(value);

            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Set,
                id: (byte)ConfigId.ModuleSerial,
                payload: payload,
                out frame);
        }

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

        public static bool ModbusMeter(byte meterIndex, bool? enabled, string serialNumber, byte? slaveAddress, uint? baudRate, byte? serialConfig, byte? readFuncCode,
            bool? forwardEnable, ushort? forwardRegAddr, byte? forwardDataType, bool? forwardWordSwap, sbyte? forwardMultiplier,
            bool? reverseEnable, ushort? reverseRegAddr, byte? reverseDataType, bool? reverseWordSwap, sbyte? reverseMultiplier,
            bool? flowEnable, ushort? flowRegAddr, byte? flowDataType, bool? flowWordSwap, sbyte? flowMultiplier, out byte[] frame)
        {
            frame = null;

            List<byte> payload = new List<byte>();
            payload.Add(meterIndex);

            if (enabled.HasValue && !enabled.Value)
            {
                payload.Add((byte)ConfigModbusMeterId.MeterEnable);
                payload.Add(0x00);

                payload.Add((byte)ConfigModbusMeterId.SerialNumber);
                for (int i = 0; i < METER_SERIAL_SIZE; i++)
                {
                    payload.Add(0x00);
                }

                payload.Add((byte)ConfigModbusMeterId.SlaveAddress);
                payload.Add(0x00);

                payload.Add((byte)ConfigModbusMeterId.Baudrate);
                payload.AddRange(new byte[sizeof(uint)]);

                payload.Add((byte)ConfigModbusMeterId.SerialConfig);
                payload.Add(0x00);

                payload.Add((byte)ConfigModbusMeterId.ReadFuncCode);
                payload.Add(0x00);

                payload.Add((byte)ConfigModbusMeterId.ForwardTotalEnable);
                payload.Add(0x00);

                payload.Add((byte)ConfigModbusMeterId.ForwardTotalRegAddr);
                payload.AddRange(new byte[sizeof(ushort)]);

                payload.Add((byte)ConfigModbusMeterId.ForwardTotalDataType);
                payload.Add(0x00);

                payload.Add((byte)ConfigModbusMeterId.ForwardTotalWordSwap);
                payload.Add(0x00);

                payload.Add((byte)ConfigModbusMeterId.ForwardTotalMultiplier);
                payload.Add(0x00);

                payload.Add((byte)ConfigModbusMeterId.ReverseTotalEnable);
                payload.Add(0x00);

                payload.Add((byte)ConfigModbusMeterId.ReverseTotalRegAddr);
                payload.AddRange(new byte[sizeof(ushort)]);

                payload.Add((byte)ConfigModbusMeterId.ReverseTotalDataType);
                payload.Add(0x00);

                payload.Add((byte)ConfigModbusMeterId.ReverseTotalWordSwap);
                payload.Add(0x00);

                payload.Add((byte)ConfigModbusMeterId.ReverseTotalMultiplier);
                payload.Add(0x00);

                payload.Add((byte)ConfigModbusMeterId.FlowRateEnable);
                payload.Add(0x00);

                payload.Add((byte)ConfigModbusMeterId.FlowRateRegAddr);
                payload.AddRange(new byte[sizeof(ushort)]);

                payload.Add((byte)ConfigModbusMeterId.FlowRateDataType);
                payload.Add(0x00);

                payload.Add((byte)ConfigModbusMeterId.FlowRateWordSwap);
                payload.Add(0x00);

                payload.Add((byte)ConfigModbusMeterId.FlowRateMultiplier);
                payload.Add(0x00);
            }
            else
            {
                if (enabled.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.MeterEnable);
                    payload.Add(enabled.Value ? (byte)1 : (byte)0);
                }

                if (!string.IsNullOrWhiteSpace(serialNumber))
                {
                    byte[] serialBytes = Encoding.ASCII.GetBytes(serialNumber.Trim());

                    if (serialBytes.Length > METER_SERIAL_SIZE)
                    {
                        return false;
                    }

                    payload.Add((byte)ConfigModbusMeterId.SerialNumber);
                    payload.AddRange(serialBytes);

                    for (int i = serialBytes.Length; i < METER_SERIAL_SIZE; i++)
                    {
                        payload.Add(0x00);
                    }
                }

                if (slaveAddress.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.SlaveAddress);
                    payload.Add(slaveAddress.Value);
                }

                if (baudRate.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.Baudrate);
                    payload.AddRange(BitConverter.GetBytes(baudRate.Value));
                }

                if (serialConfig.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.SerialConfig);
                    payload.Add(serialConfig.Value);
                }

                if (readFuncCode.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.ReadFuncCode);
                    payload.Add(readFuncCode.Value);
                }

                if (forwardEnable.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.ForwardTotalEnable);
                    payload.Add(forwardEnable.Value ? (byte)1 : (byte)0);
                }

                if (forwardRegAddr.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.ForwardTotalRegAddr);
                    payload.AddRange(BitConverter.GetBytes(forwardRegAddr.Value));
                }

                if (forwardDataType.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.ForwardTotalDataType);
                    payload.Add(forwardDataType.Value);
                }

                if (forwardWordSwap.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.ForwardTotalWordSwap);
                    payload.Add(forwardWordSwap.Value ? (byte)1 : (byte)0);
                }

                if (forwardMultiplier.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.ForwardTotalMultiplier);
                    payload.Add(unchecked((byte)forwardMultiplier.Value));
                }

                if (reverseEnable.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.ReverseTotalEnable);
                    payload.Add(reverseEnable.Value ? (byte)1 : (byte)0);
                }

                if (reverseRegAddr.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.ReverseTotalRegAddr);
                    payload.AddRange(BitConverter.GetBytes(reverseRegAddr.Value));
                }

                if (reverseDataType.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.ReverseTotalDataType);
                    payload.Add(reverseDataType.Value);
                }

                if (reverseWordSwap.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.ReverseTotalWordSwap);
                    payload.Add(reverseWordSwap.Value ? (byte)1 : (byte)0);
                }

                if (reverseMultiplier.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.ReverseTotalMultiplier);
                    payload.Add(unchecked((byte)reverseMultiplier.Value));
                }

                if (flowEnable.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.FlowRateEnable);
                    payload.Add(flowEnable.Value ? (byte)1 : (byte)0);
                }

                if (flowRegAddr.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.FlowRateRegAddr);
                    payload.AddRange(BitConverter.GetBytes(flowRegAddr.Value));
                }

                if (flowDataType.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.FlowRateDataType);
                    payload.Add(flowDataType.Value);
                }

                if (flowWordSwap.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.FlowRateWordSwap);
                    payload.Add(flowWordSwap.Value ? (byte)1 : (byte)0);
                }

                if (flowMultiplier.HasValue)
                {
                    payload.Add((byte)ConfigModbusMeterId.FlowRateMultiplier);
                    payload.Add(unchecked((byte)flowMultiplier.Value));
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
                id: (byte)ConfigId.ModbusMeter,
                payload: payload.ToArray(),
                out frame);
        }

        public static bool PressureSensor(byte meterIndex, bool? enabled, string serialNumber, float? minCurrent, float? maxCurrent, float? minPressure, float? maxPressure, out byte[] frame)
        {
            frame = null;

            List<byte> payload = new List<byte>();
            payload.Add(meterIndex);

            if (enabled.HasValue && !enabled.Value)
            {
                payload.Add((byte)ConfigPressureSensorId.MeterEnable);
                payload.Add(0x00);

                payload.Add((byte)ConfigPressureSensorId.SerialNumber);
                payload.AddRange(new byte[METER_SERIAL_SIZE]);

                payload.Add((byte)ConfigPressureSensorId.MinCurrent);
                payload.AddRange(new byte[sizeof(float)]);

                payload.Add((byte)ConfigPressureSensorId.MaxCurrent);
                payload.AddRange(new byte[sizeof(float)]);

                payload.Add((byte)ConfigPressureSensorId.MinPressure);
                payload.AddRange(new byte[sizeof(float)]);

                payload.Add((byte)ConfigPressureSensorId.MaxPressure);
                payload.AddRange(new byte[sizeof(float)]);
            }
            else
            {
                if (enabled.HasValue)
                {
                    payload.Add((byte)ConfigPressureSensorId.MeterEnable);
                    payload.Add(enabled.Value ? (byte)1 : (byte)0);
                }

                if (serialNumber != null)
                {
                    payload.Add((byte)ConfigPressureSensorId.SerialNumber);

                    byte[] serialBytes = Encoding.ASCII.GetBytes(serialNumber);
                    Array.Resize(ref serialBytes, METER_SERIAL_SIZE);
                    payload.AddRange(serialBytes);
                }

                if (minCurrent.HasValue)
                {
                    payload.Add((byte)ConfigPressureSensorId.MinCurrent);
                    payload.AddRange(BitConverter.GetBytes(minCurrent.Value));
                }

                if (maxCurrent.HasValue)
                {
                    payload.Add((byte)ConfigPressureSensorId.MaxCurrent);
                    payload.AddRange(BitConverter.GetBytes(maxCurrent.Value));
                }

                if (minPressure.HasValue)
                {
                    payload.Add((byte)ConfigPressureSensorId.MinPressure);
                    payload.AddRange(BitConverter.GetBytes(minPressure.Value));
                }

                if (maxPressure.HasValue)
                {
                    payload.Add((byte)ConfigPressureSensorId.MaxPressure);
                    payload.AddRange(BitConverter.GetBytes(maxPressure.Value));
                }
            }

            if (payload.Count <= 1)
                return false;

            return Pack(true, PROTOCOL_MODULE_SERIAL_COMMON, (byte)CmdCode.Set, (byte)ConfigId.PressureSensor, payload.ToArray(), out frame);
        }

        public static bool Reboot(out byte[] frame)
        {
            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Set,
                id: (byte)ConfigId.Reboot,
                payload: null,
                out frame);
        }

        public static bool ResetSetting(out byte[] frame)
        {
            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Set,
                id: (byte)ConfigId.ResetSetting,
                payload: null,
                out frame);
        }

        public static bool ChangePassword(string currentPassword, byte role, string newPassword, out byte[] frame)
        {
            frame = null;

            byte[] currentPasswordBytes = Encoding.ASCII.GetBytes(currentPassword);
            byte[] newPasswordBytes = Encoding.ASCII.GetBytes(newPassword);

            if (currentPasswordBytes.Length != PASSWORD_LENGTH || newPasswordBytes.Length != PASSWORD_LENGTH)
            {
                return false;
            }

            byte[] payload = new byte[PASSWORD_LENGTH + sizeof(byte) + PASSWORD_LENGTH];

            int index = 0;

            Array.Copy(currentPasswordBytes, 0, payload, index, PASSWORD_LENGTH);
            index += PASSWORD_LENGTH;

            payload[index++] = role;

            Array.Copy(newPasswordBytes, 0, payload, index, PASSWORD_LENGTH);

            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Set,
                id: (byte)ConfigId.ChangePassword,
                payload: payload,
                out frame);
        }

        public static bool ResetPassword(out byte[] frame)
        {
            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Set,
                id: (byte)ConfigId.ResetPassword,
                payload: null,
                out frame);
        }

        public static bool McuResetCount(string count, out byte[] frame)
        {
            frame = null;

            if (string.IsNullOrWhiteSpace(count))
                return false;

            byte value = byte.Parse(count);

            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Set,
                id: (byte)ConfigId.McuResetCount,
                payload: new byte[] { value },
                out frame);
        }

        public static bool EraseLatchData(out byte[] frame)
        {
            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Set,
                id: (byte)ConfigId.EraseMeasurementData,
                payload: null,
                out frame);
        }

        public static bool EraseEventData(out byte[] frame)
        {
            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Set,
                id: (byte)ConfigId.EraseEventData,
                payload: null,
                out frame);
        }

        public static bool EraseErrLog(out byte[] frame)
        {
            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Set,
                id: (byte)ConfigId.EraseLogData,
                payload: null,
                out frame);
        }

        public static bool LatchActive(out byte[] frame)
        {
            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Set,
                id: (byte)ConfigId.LatchImmediately,
                payload: null,
                out frame);
        }

        public static bool EventCreate(byte meterType, byte meterIndex, byte eventCode, out byte[] frame)
        {
            byte[] payload =
            {
                meterType,
                meterIndex,
                eventCode
            };

            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Set,
                id: (byte)ConfigId.EventCreate,
                payload: payload,
                out frame);
        }

        public static bool PushActive(out byte[] frame)
        {
            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Set,
                id: (byte)ConfigId.PushImmediately,
                payload: null,
                out frame);
        }

        public static bool FactoryReset(out byte[] frame)
        {
            return Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Set,
                id: (byte)ConfigId.FactoryReset,
                payload: null,
                out frame);
        }
    }
}
