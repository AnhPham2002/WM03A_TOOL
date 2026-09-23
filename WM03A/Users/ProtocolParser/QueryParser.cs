using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WM03A.Users.Model;
using static WM03A.Protocol;

namespace WM03A
{
    internal class QueryParser
    {
        private const int MAX_PULSE_METER_COUNT = 4;
        private const int MAX_MODBUS_METER_COUNT = 4;
        private const int MAX_PRESSURE_SENSOR_COUNT = 2;

        private const int MAX_PULSE_GATE_COUNT = 4;
        private const int EEPROM_METADATA_SIZE = 16;
        private const int PULSE_COUNT_SIZE = 16;

        public struct MeterData
        {
            public byte[] MeterSerial;
            public double ForwardTotalizer;
            public double ReverseTotalizer;
            public double FlowRate;
        }

        public static bool SimNetworkInfo(byte[] frame, out byte[] ccid, out sbyte rssi, out sbyte rsrp, out sbyte rsrq, out sbyte rssnr)
        {
            ccid = null;
            rssi = default;
            rsrp = default;
            rsrq = default;
            rssnr = default;
            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);
            if (payload.Length < 29)
                return false;
            ccid = payload.Skip(0).Take(25).ToArray();
            rssi = unchecked((sbyte)payload[25]);
            rsrp = unchecked((sbyte)payload[26]);
            rsrq = unchecked((sbyte)payload[27]);
            rssnr = unchecked((sbyte)payload[28]);
            return true;
        }

        public static bool PulseMeterData(byte[] frame, out MeterData data)
        {
            data = default;
            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);
            if (payload.Length < 44)
                return false;
            data.MeterSerial = payload.Skip(0).Take(20).ToArray();
            data.ForwardTotalizer = BitConverter.ToDouble(payload, 20);
            data.ReverseTotalizer = BitConverter.ToDouble(payload, 28);
            data.FlowRate = BitConverter.ToDouble(payload, 36);
            return true;
        }

        public static bool ModbusMeterData(byte[] frame, out MeterData data)
        {
            data = default;
            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);
            if (payload.Length < 44)
                return false;
            data.MeterSerial = payload.Skip(0).Take(20).ToArray();
            data.ForwardTotalizer = BitConverter.ToDouble(payload, 20);
            data.ReverseTotalizer = BitConverter.ToDouble(payload, 28);
            data.FlowRate = BitConverter.ToDouble(payload, 36);
            return true;
        }

        public static bool PressureSensorData(byte[] frame, out byte[] meterSerial, out float data)
        {
            meterSerial = default;
            data = default;
            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);
            if (payload.Length < 24)
                return false;
            meterSerial = payload.Skip(0).Take(20).ToArray();
            data = BitConverter.ToSingle(payload, 20);
            return true;
        }

        public static bool Latch(byte[] frame, out LatchData data)
        {
            data = new LatchData();

            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);

            if (payload == null || payload.Length < 12)
            {
                return false;
            }

            int index = 0;

            try
            {
                byte currentYear = payload[index++];
                byte currentMonth = payload[index++];
                byte currentDate = payload[index++];
                byte currentHours = payload[index++];
                byte currentMinutes = payload[index++];
                byte currentSeconds = payload[index++];

                data.CurrentDateTime = new DateTime(2000 + currentYear, currentMonth, currentDate, currentHours, currentMinutes, currentSeconds);

                byte recordYear = payload[index++];
                byte recordMonth = payload[index++];
                byte recordDate = payload[index++];
                byte recordHours = payload[index++];
                byte recordMinutes = payload[index++];
                byte recordSeconds = payload[index++];

                data.LatchDateTime = new DateTime(2000 + recordYear, recordMonth, recordDate, recordHours, recordMinutes, recordSeconds);

                while (index < payload.Length)
                {
                    if (index + 2 > payload.Length)
                    {
                        return false;
                    }

                    byte meterType = payload[index++];
                    byte serialLength = payload[index++];

                    if (index + serialLength > payload.Length)
                    {
                        return false;
                    }

                    string serialNumber = Encoding.ASCII.GetString(payload, index, serialLength);
                    index += serialLength;

                    LatchMeterData meterData = new LatchMeterData
                    {
                        MeterType = meterType,
                        SerialNumber = serialNumber
                    };

                    switch (meterType)
                    {
                        case (byte)MeterType.PulseMeterType:
                        case (byte)MeterType.ModbusMeterType:
                            {
                                if (index + 3 * sizeof(double) > payload.Length)
                                {
                                    return false;
                                }

                                meterData.ForwardTotalizer = BitConverter.ToDouble(payload, index);
                                index += sizeof(double);

                                meterData.ReverseTotalizer = BitConverter.ToDouble(payload, index);
                                index += sizeof(double);

                                meterData.FlowRate = BitConverter.ToDouble(payload, index);
                                index += sizeof(double);

                                break;
                            }

                        case (byte)MeterType.PressureSensorType:
                            {
                                if (index + sizeof(float) > payload.Length)
                                {
                                    return false;
                                }

                                meterData.Pressure = BitConverter.ToSingle(payload, index);
                                index += sizeof(float);

                                break;
                            }

                        default:
                            return false;
                    }

                    data.Meters.Add(meterData);
                }
            }
            catch
            {
                return false;
            }

            return index == payload.Length;
        }

        public static bool Event(byte[] frame, out EventData data)
        {
            data = null;

            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);

            if (payload == null || payload.Length < 15)
            {
                return false;
            }

            int index = 0;

            byte currentYear = payload[index++];
            byte currentMonth = payload[index++];
            byte currentDate = payload[index++];
            byte currentHours = payload[index++];
            byte currentMinutes = payload[index++];
            byte currentSeconds = payload[index++];

            DateTime currentDateTime = new DateTime(2000 + currentYear, currentMonth, currentDate, currentHours, currentMinutes, currentSeconds);

            byte eventYear = payload[index++];
            byte eventMonth = payload[index++];
            byte eventDate = payload[index++];
            byte eventHours = payload[index++];
            byte eventMinutes = payload[index++];
            byte eventSeconds = payload[index++];

            DateTime eventDateTime = new DateTime(2000 + eventYear, eventMonth, eventDate, eventHours, eventMinutes, eventSeconds);

            byte meterType = payload[index++];
            byte serialLength = payload[index++];

            if (index + serialLength + 1 > payload.Length)
            {
                return false;
            }

            string serialNumber = Encoding.ASCII.GetString(payload, index, serialLength);
            index += serialLength;

            byte eventCode = payload[index++];

            if (index != payload.Length)
            {
                return false;
            }

            data = new EventData
            {
                CurrentDateTime = currentDateTime,
                EventDateTime = eventDateTime,
                MeterType = meterType,
                SerialNumber = serialNumber,
                EventCode = eventCode
            };

            return true;
        }

        public static bool Metadata(byte[] frame, out MetadataData data)
        {
            data = null;

            Unpack(frame, out ulong serial, out byte cmd, out byte id, out byte[] payload);

            if (payload == null)
            {
                return false;
            }

            int expectedLength = sizeof(ulong) + EEPROM_METADATA_SIZE + sizeof(ulong) + sizeof(byte) + (MAX_PULSE_GATE_COUNT * PULSE_COUNT_SIZE);

            if (payload.Length != expectedLength)
            {
                return false;
            }

            int index = 0;

            ulong sequenceMeta = BitConverter.ToUInt64(payload, index);
            index += sizeof(ulong);

            ushort nextLatchSaveIndex = BitConverter.ToUInt16(payload, index);
            index += sizeof(ushort);

            ushort nextLatchLoadIndex = BitConverter.ToUInt16(payload, index);
            index += sizeof(ushort);

            ushort latchCount = BitConverter.ToUInt16(payload, index);
            index += sizeof(ushort);

            ushort nextEventSaveIndex = BitConverter.ToUInt16(payload, index);
            index += sizeof(ushort);

            ushort nextEventLoadIndex = BitConverter.ToUInt16(payload, index);
            index += sizeof(ushort);

            ushort eventCount = BitConverter.ToUInt16(payload, index);
            index += sizeof(ushort);

            ushort nextLogSaveIndex = BitConverter.ToUInt16(payload, index);
            index += sizeof(ushort);

            ushort logCount = BitConverter.ToUInt16(payload, index);
            index += sizeof(ushort);

            ulong sequenceRuntime = BitConverter.ToUInt64(payload, index);
            index += sizeof(ulong);

            byte resetCount = payload[index++];

            var pulseCounts = new List<PulseCountData>();

            for (int i = 0; i < MAX_PULSE_GATE_COUNT; i++)
            {
                ulong forwardPulseCount = BitConverter.ToUInt64(payload, index);
                index += sizeof(ulong);

                ulong reversePulseCount = BitConverter.ToUInt64(payload, index);
                index += sizeof(ulong);

                pulseCounts.Add(new PulseCountData
                {
                    ForwardPulseCount = forwardPulseCount,
                    ReversePulseCount = reversePulseCount
                });
            }

            if (index != payload.Length)
            {
                return false;
            }

            data = new MetadataData
            {
                SequenceMeta = sequenceMeta,
                NextLatchSaveIndex = nextLatchSaveIndex,
                NextLatchLoadIndex = nextLatchLoadIndex,
                LatchCount = latchCount,
                NextEventSaveIndex = nextEventSaveIndex,
                NextEventLoadIndex = nextEventLoadIndex,
                EventCount = eventCount,
                NextLogSaveIndex = nextLogSaveIndex,
                LogCount = logCount,
                SequenceRuntime = sequenceRuntime,
                ResetCount = resetCount,
                PulseCounts = pulseCounts
            };

            return true;
        }
    }
}
