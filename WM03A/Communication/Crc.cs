using System;

namespace WM03A
{
    public static class Crc
    {
        public static ushort CalculateCrc16(byte[] pData, ushort u16Size)
        {
            ushort u16Crc = 0xFFFF;

            for (int u16Pos = 0; u16Pos < u16Size; u16Pos++)
            {
                u16Crc ^= pData[u16Pos];

                for (int i = 8; i != 0; i--)
                {
                    if ((u16Crc & 0x0001) != 0)
                    {
                        u16Crc = (ushort)((u16Crc >> 1) ^ 0xA001);
                    }
                    else
                    {
                        u16Crc >>= 1;
                    }
                }
            }

            return u16Crc;
        }

        public static uint CalculateCrc32(byte[] data, uint init = 0xFFFFFFFF)
        {
            uint poly = 0x04C11DB7;
            uint crc = init & 0xFFFFFFFF;

            uint CrcWordUpdate(uint crcVal, uint word)
            {
                crcVal ^= word;
                for (int j = 0; j < 32; j++)
                {
                    if ((crcVal & 0x80000000) != 0)
                    {
                        crcVal = (uint)((crcVal << 1) ^ poly);
                    }
                    else
                    {
                        crcVal <<= 1;
                    }
                }
                return crcVal;
            }

            int n = data.Length;
            int i = 0;
            while (i + 4 <= n)
            {
                uint w = ((uint)data[i] << 24) | ((uint)data[i + 1] << 16) | ((uint)data[i + 2] << 8) | data[i + 3];
                crc = CrcWordUpdate(crc, w);
                i += 4;
            }

            int rem = n - i;
            if (rem > 0)
            {
                uint b0 = rem >= 1 ? data[i] : 0U;
                uint b1 = rem >= 2 ? data[i + 1] : 0U;
                uint b2 = rem >= 3 ? data[i + 2] : 0U;
                uint b3 = 0U;
                uint w = (b0 << 24) | (b1 << 16) | (b2 << 8) | b3;
                crc = CrcWordUpdate(crc, w);
            }

            return crc & 0xFFFFFFFF;
        }
    }
}
