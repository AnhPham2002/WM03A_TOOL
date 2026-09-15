using System;
using System.Security.Cryptography;

namespace WM03A
{
    public static class Aes128
    {
        /// <summary>
        /// Encrypt data using AES-128 ECB + Zero padding.
        /// Tương đương: void aes_128_encrypt(const uint8_t au8Key[16], uint8_t *pData, uint16_t u16DataSize);
        /// </summary>
        /// <param name="key">Khóa 16 byte</param>
        /// <param name="data">Buffer chứa dữ liệu (sẽ bị ghi đè kết quả mã hóa)</param>
        /// <param name="dataSize">Độ dài dữ liệu gốc (trước khi padding)</param>
        public static void Encrypt(byte[] key, byte[] data, ushort dataSize)
        {
            if (key == null || key.Length != 16)
                throw new ArgumentException("Key must be 16 bytes", nameof(key));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // Số block (ceil)
            int blockNum = (dataSize + 15) / 16;
            int paddedLen = blockNum * 16;

            if (data.Length < paddedLen)
                throw new ArgumentException($"Data buffer must be at least {paddedLen} bytes", nameof(data));

            // Zero padding
            for (int i = dataSize; i < paddedLen; i++)
            {
                data[i] = 0x00;
            }

            // Encrypt từng block
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;

                using (var encryptor = aes.CreateEncryptor())
                {
                    for (int i = 0; i < blockNum; i++)
                    {
                        encryptor.TransformBlock(data, i * 16, 16, data, i * 16);
                    }
                }
            }
        }

        /// <summary>
        /// Decrypt data using AES-128 ECB.
        /// Tương đương: void aes_128_decrypt(const uint8_t au8Key[16], uint8_t *pData, uint16_t u16DataSize);
        /// </summary>
        /// <param name="key">Khóa 16 byte</param>
        /// <param name="data">Buffer chứa ciphertext (sẽ bị ghi đè kết quả giải mã)</param>
        /// <param name="dataSize">Độ dài dữ liệu (phải là bội số của 16)</param>
        public static void Decrypt(byte[] key, byte[] data, ushort dataSize)
        {
            if (key == null || key.Length != 16)
                throw new ArgumentException("Key must be 16 bytes", nameof(key));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // Số block (ceil)
            int blockNum = (dataSize + 15) / 16;
            int processLen = blockNum * 16;

            if (data.Length < processLen)
                throw new ArgumentException($"Data buffer must be at least {processLen} bytes", nameof(data));

            // Decrypt từng block
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;

                using (var decryptor = aes.CreateDecryptor())
                {
                    for (int i = 0; i < blockNum; i++)
                    {
                        decryptor.TransformBlock(data, i * 16, 16, data, i * 16);
                    }
                }
            }
        }
    }
}