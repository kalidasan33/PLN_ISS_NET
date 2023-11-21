using System;
using System.Text;
using System.Security.Cryptography;

namespace ISS.Security
{
    /// <summary>
    /// Utility class for encryption / decryption / hashing operations
    /// </summary>
    public class EncryptionService
    {
        static readonly TripleDESCryptoServiceProvider DesCryptoService = new TripleDESCryptoServiceProvider();
        static readonly MD5CryptoServiceProvider Md5CryptoService = new MD5CryptoServiceProvider();
        private const string Key = "ReferenceKey";

        /// <summary>
        /// Encrypts the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string Encrypt(string text)
        {
            DesCryptoService.Key = Md5CryptoService.ComputeHash(Encoding.ASCII.GetBytes(Key));
            DesCryptoService.Mode = CipherMode.ECB;
            var encryptor = DesCryptoService.CreateEncryptor();
            var buff = Encoding.ASCII.GetBytes(text);
            var encryptedText = Convert.ToBase64String(encryptor.TransformFinalBlock(buff, 0, buff.Length));
            encryptedText = encryptedText.Replace("+", "!").Replace("/", "_");
            return encryptedText;
        }

        /// <summary>
        /// Decypts the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string Decypt(string text)
        {
            text = text.Replace("!", "+").Replace("_", "/");
            DesCryptoService.Key = Md5CryptoService.ComputeHash(Encoding.ASCII.GetBytes(Key));
            DesCryptoService.Mode = CipherMode.ECB;
            var decryptor = DesCryptoService.CreateDecryptor();
            var buf = Convert.FromBase64String(text);
            var decryptedText = Encoding.ASCII.GetString(decryptor.TransformFinalBlock(buf, 0, buf.Length));
            return decryptedText;
        }

        /// <summary>
        /// Gets the hash.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static HashSalt GetHash(string text)
        {
            return HashGenerator.GetHash(text);
        }
    }
}
