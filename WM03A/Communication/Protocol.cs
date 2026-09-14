using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace WM03A
{
    public static class Protocol
    {
        public const byte PROTOCOL_START_OF_FRAME = 0x5A;
        public const byte PROTOCOL_MODULE_TYPE = 0x03;
        public const byte PROTOCOL_MODULE_SERIAL_COMMON = 0x00;

        // Key mặc định
        private static readonly byte[] AesKey = new byte[]
        {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
            0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
        };

        private static int Align16(int x) => (x + 15) & ~15;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ProtocolHeader
        {
            public byte StartOfFrame;      // 1
            public byte ModuleType;        // 1
            public ulong ModuleSerial;     // 8
            public byte CmdCode;           // 1
            public byte IdCode;            // 1
            public ushort CiphertextLen;   // 2
            // Tổng = 14 bytes
        }

        public enum ProtocolErrCode : byte
        {
            Success = 0x00,
            CmdInvalid = 0x01,
            Crc16Invalid = 0x02,
            FrameInvalid = 0x03,
            FwNoSpace = 0x04,
            FwCrc32Failed = 0x05,
            FwIncomplete = 0x06,
            FlashEraseFailed = 0x07,
            FlashWriteFailed = 0x08,
            PasswordIncorrect = 0x10,
            AccessDenied = 0x11,
            ModuleSerialInvalid = 0xFD,
            NullPointer = 0xFE,
            Unknown = 0xFF
        }

        // ============================================================
        // Command codes
        // ============================================================
        public enum CmdCode : byte
        {
            Access = 0x00,
            Get = 0x01,
            Set = 0x02,
            Query = 0x03,
            Push = 0x04,
            Ota = 0x05
        }

        // ============================================================
        // ID for CMD_ACCESS (0x00)
        // ============================================================
        public enum AccessId : byte
        {
            User = 0x00,
            Technician = 0x01,
            Admin = 0x02
        }

        // ============================================================
        // ID for CMD_GET & CMD_SET (0x01 & 0x02)
        // ============================================================
        public enum ConfigId : byte
        {
            ModuleSerial = 0x00,
            Time = 0x01,
            IpEndpoint = 0x02,
            Module = 0x03,
            PulseMeter = 0x04,
            ModbusMeter = 0x05,
            PressureSensor = 0x06,
            Reboot = 0x07,
            ResetSetting = 0x08,
            ChangePassword = 0x09,
            ResetPassword = 0x10,
            McuResetCount = 0x11,
            EraseMeasurementData = 0x12,
            EraseEventData = 0x13,
            EraseLogData = 0x14,
            FactoryReset = 0x15,
            LatchImmediately = 0x16,
            PushImmediately = 0x17,
            EventCreate = 0x18
        }

        // ============================================================
        // ID for CMD_QUERY (0x03)
        // ============================================================
        public enum QueryId : byte
        {
            BootloaderVersion = 0x00,
            FirmwareVersion = 0x01,
            PowerSupplyInfo = 0x02,
            SimNetworkInfo = 0x03,
            Latch = 0x04,
            Event = 0x05,
            Log = 0x06
        }

        // ============================================================
        // ID for CMD_PUSH (0x04)
        // ============================================================
        public enum PushId : byte
        {
            Info = 0x00,
            Latch = 0x01,
            Event = 0x02,
            ModuleConfig = 0x03,
            PulseMeterConfig = 0x04,
            ModbusMeterConfig = 0x05,
            PressureSensorConfig = 0x06
        }

        // ============================================================
        // ID for CMD_OTA (0x05)
        // ============================================================
        public enum OtaId : byte
        {
            UpdateRequest = 0x00,
            SendInfo = 0x01,
            SendPacket = 0x02
        }


        // ============================================================
        // PUBLIC API
        // ============================================================

        public static bool Pack(bool encrypt, ulong serial, byte cmd, byte id,
                        byte[] payload, out byte[] frame)
        {
            frame = null;

            int headerSize = Marshal.SizeOf<ProtocolHeader>(); // 14 

            ushort ciphertextLen;
            byte[] ciphertextBuf;
            ushort ciphertextCrc;

            if (payload == null || payload.Length == 0)
            {
                ciphertextLen = 0;
                ciphertextBuf = Array.Empty<byte>();
            }
            else
            {
                // Plaintext = 2 bytes length + payload + 2 bytes inner CRC16 
                int plaintextLen = sizeof(ushort) + payload.Length + sizeof(ushort);
                ciphertextBuf = new byte[Align16(plaintextLen)]; // đủ chỗ cho padding 

                // Copy length (little-endian) 
                BitConverter.GetBytes((ushort)payload.Length).CopyTo(ciphertextBuf, 0);
                // Copy payload 
                Buffer.BlockCopy(payload, 0, ciphertextBuf, sizeof(ushort), payload.Length);
                // Calculate and copy inner CRC16 
                ciphertextCrc = Crc.CalculateCrc16(ciphertextBuf, (ushort)(sizeof(ushort) + payload.Length));
                BitConverter.GetBytes(ciphertextCrc).CopyTo(ciphertextBuf, sizeof(ushort) + payload.Length);

                if (encrypt)
                {
                    ciphertextLen = (ushort)Align16(plaintextLen);
                    Aes128.Encrypt(AesKey, ciphertextBuf, (ushort)plaintextLen);
                }
                else
                {
                    ciphertextLen = (ushort)plaintextLen;
                }
            }

            // Frame = Header + Ciphertext + CRC16 
            int packetLen = headerSize + ciphertextLen;
            frame = new byte[packetLen + sizeof(ushort)];

            // Ghi header 
            var header = new ProtocolHeader
            {
                StartOfFrame = PROTOCOL_START_OF_FRAME,
                ModuleType = PROTOCOL_MODULE_TYPE,
                ModuleSerial = serial,
                CmdCode = cmd,
                IdCode = id,
                CiphertextLen = ciphertextLen
            };

            byte[] headerBytes = StructureToBytes(header);
            Buffer.BlockCopy(headerBytes, 0, frame, 0, headerSize);

            // Copy ciphertext 
            if (ciphertextLen > 0)
            {
                Buffer.BlockCopy(ciphertextBuf, 0, frame, headerSize, ciphertextLen);
            }

            // CRC16 trên Header + Ciphertext 
            ushort crc = Crc.CalculateCrc16(frame, (ushort)packetLen);
            BitConverter.GetBytes(crc).CopyTo(frame, packetLen);

            return true;
        }

        public static ProtocolErrCode Unpack(byte[] frame,
                                             out ulong serial, out byte cmd, out byte id,
                                             out byte[] payload)
        {
            serial = PROTOCOL_MODULE_SERIAL_COMMON;
            cmd = 0;
            id = 0;
            payload = null;

            if (frame == null)
                return ProtocolErrCode.NullPointer;

            int headerSize = Marshal.SizeOf<ProtocolHeader>();

            if (frame.Length < headerSize + 2)
                return ProtocolErrCode.FrameInvalid;

            var header = BytesToStructure<ProtocolHeader>(frame, 0);

            if (header.StartOfFrame != PROTOCOL_START_OF_FRAME)
                return ProtocolErrCode.FrameInvalid;

            if (header.CiphertextLen > frame.Length - headerSize - sizeof(ushort))
                return ProtocolErrCode.FrameInvalid;

            int packetLen = frame.Length - sizeof(ushort);

            // Kiểm tra CRC 
            ushort crcFrameCalc = Crc.CalculateCrc16(frame, (ushort)packetLen);
            ushort crcFrame = BitConverter.ToUInt16(frame, packetLen);

            if (crcFrameCalc != crcFrame)
                return ProtocolErrCode.Crc16Invalid;

            serial = header.ModuleSerial;
            cmd = header.CmdCode;
            id = header.IdCode;

            if (header.CiphertextLen == 0)
            {
                payload = Array.Empty<byte>();
                return ProtocolErrCode.Success;
            }

            // Copy ciphertext ra buffer tạm 
            byte[] temp = new byte[header.CiphertextLen];
            Buffer.BlockCopy(frame, headerSize, temp, 0, header.CiphertextLen);

            // Decrypt nếu >= 16 byte 
            if (header.CiphertextLen >= 16)
            {
                Aes128.Decrypt(AesKey, temp, header.CiphertextLen);
            }

            // Lấy payload length (2 byte đầu) 
            if (temp.Length < sizeof(ushort))
                return ProtocolErrCode.FrameInvalid;

            ushort payloadLen = BitConverter.ToUInt16(temp, 0);

            // Bảo vệ overflow 
            if (payloadLen >= header.CiphertextLen)
                return ProtocolErrCode.FrameInvalid;

            // Kiểm tra inner CRC16 
            ushort crcInnerCalc = Crc.CalculateCrc16(temp, (ushort)(sizeof(ushort) + payloadLen));
            ushort crcInner = BitConverter.ToUInt16(temp, sizeof(ushort) + payloadLen);
            if (crcInnerCalc != crcInner)
                return ProtocolErrCode.FrameInvalid;

            // Copy payload thật 
            payload = new byte[payloadLen];
            Buffer.BlockCopy(temp, sizeof(ushort), payload, 0, payloadLen);

            return ProtocolErrCode.Success;
        }

        /// <summary>
        /// Pack ACK frame: DateTime (6 bytes) + ErrorCode (1 byte)
        /// </summary>
        private static bool PackAck(byte cmd, byte id, Protocol.ProtocolErrCode errCode, out byte[] frame)
        {
            // Payload: Year, Month, Day, Hour, Minute, Second, ErrorCode
            byte[] payload = new byte[7];

            DateTime now = DateTime.Now;

            payload[0] = (byte)(now.Year % 100);   // 2026 → 26
            payload[1] = (byte)now.Month;
            payload[2] = (byte)now.Day;
            payload[3] = (byte)now.Hour;
            payload[4] = (byte)now.Minute;
            payload[5] = (byte)now.Second;
            payload[6] = (byte)errCode;

            // Serial = 0 như bạn yêu cầu
            ulong serial = PROTOCOL_MODULE_SERIAL_COMMON;

            return Protocol.Pack(
                encrypt: false,
                serial: serial,
                cmd: cmd,
                id: id,
                payload: payload,
                out frame);
        }

        /// <summary>
        /// Pack lệnh Access (CMD = 0x00)
        /// </summary>
        /// <param name="id">Access level (Lv2 / Lv3 / Lv4)</param>
        /// <param name="password">Mật khẩu</param>
        /// <param name="frame">Frame kết quả</param>
        public static bool Access(AccessId id, byte[] password, out byte[] frame)
        {
            return Pack(
                encrypt: true,                  // Access thường được mã hóa
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Access,
                id: (byte)id,
                payload: password ?? Array.Empty<byte>(),
                out frame);
        }

        // ============================================================
        // Helper
        // ============================================================

        private static byte[] StructureToBytes<T>(T structure) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structure, ptr, false);
                Marshal.Copy(ptr, arr, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return arr;
        }

        private static T BytesToStructure<T>(byte[] bytes, int offset) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, offset, ptr, size);
                return Marshal.PtrToStructure<T>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}