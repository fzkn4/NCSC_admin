using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NCSC
{
    public static class EncryptionHelper
    {
        private static readonly string key = "A3D5!8s9Kf@7LzX1"; // Must be 16, 24 or 32 chars
        private static readonly string iv = "Z4R7f$1cWq#9LdN8";  // Must be 16 chars

        public static string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cs))
                {
                    writer.Write(plainText);
                    writer.Close();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string encryptedText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);

                byte[] buffer = Convert.FromBase64String(encryptedText);

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(buffer))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
