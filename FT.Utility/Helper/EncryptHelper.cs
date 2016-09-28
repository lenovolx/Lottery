namespace FT.Utility.Helper
{
    using System;
    using System.Text;
    using System.Security.Cryptography;
    using System.IO;
    public class EncryptHelper
    {
        private const string Vector = "YBSC!@#$%^&*()12";
        private const string Key = "ZD4417JEFFDDSCC50H3FAE3C787D0E23";
        /// <summary>  
        /// AES加密  
        /// </summary>  
        /// <param name="data">被加密的明文</param>  
        /// <param name="key">密钥</param>  
        /// <returns>密文</returns>  
        public static string AesEncrypt(string data)
        {
            var plainBytes = Encoding.UTF8.GetBytes(data);

            var bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);
            var bVector = new byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(Vector.PadRight(bVector.Length)), bVector, bVector.Length);

            byte[] cryptograph; // 加密后的密文  

            var aes = Rijndael.Create();
            try
            {
                // 开辟一块内存流  
                using (var memory = new MemoryStream())
                {
                    // 把内存流对象包装成加密流对象  
                    using (var encryptor = new CryptoStream(memory,
                     aes.CreateEncryptor(bKey, bVector),
                     CryptoStreamMode.Write))
                    {
                        // 明文数据写入加密流  
                        encryptor.Write(plainBytes, 0, plainBytes.Length);
                        encryptor.FlushFinalBlock();

                        cryptograph = memory.ToArray();
                    }
                }
            }
            catch
            {
                cryptograph = null;
            }

            return Convert.ToBase64String(cryptograph);
        }

        /// <summary>  
        /// AES解密  
        /// </summary>  
        /// <param name="data">被解密的密文</param>
        /// <param name="key"></param>
        /// <returns>明文</returns>  
        public static string AesDecrypt(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                var encryptedBytes = Convert.FromBase64String(data);
                var bKey = new byte[32];
                Array.Copy(Encoding.UTF8.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);
                var bVector = new byte[16];
                Array.Copy(Encoding.UTF8.GetBytes(Vector.PadRight(bVector.Length)), bVector, bVector.Length);
                byte[] original; // 解密后的明文  
                var aes = Rijndael.Create();
                try
                {
                    // 开辟一块内存流，存储密文  
                    using (var memory = new MemoryStream(encryptedBytes))
                    {
                        // 把内存流对象包装成加密流对象  
                        using (var decryptor = new CryptoStream(memory,
                            aes.CreateDecryptor(bKey, bVector),
                            CryptoStreamMode.Read))
                        {
                            // 明文存储区  
                            using (var originalMemory = new MemoryStream())
                            {
                                var buffer = new byte[1024];
                                int readBytes;
                                while ((readBytes = decryptor.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    originalMemory.Write(buffer, 0, readBytes);
                                }

                                original = originalMemory.ToArray();
                            }
                        }
                    }
                }
                catch
                {
                    original = null;
                }
                return Encoding.UTF8.GetString(original);
            }
            throw new Exception("解密数据为空");
        }

    }
}
