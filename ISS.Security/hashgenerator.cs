using System;
using System.Security.Cryptography;

namespace ISS.Security
{
    /// <summary>
    /// Utility class to generate hash and salt values
    /// </summary>
    internal class HashGenerator
    {
        static readonly MD5CryptoServiceProvider Md5CryptoService = new MD5CryptoServiceProvider();
        static readonly RNGCryptoServiceProvider RngCryptoService = new RNGCryptoServiceProvider();
        private const int SaltSize = 24;

        /// <summary>
        /// Gets the hash.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static HashSalt GetHash(string text)
        {
            var hSalt = new HashSalt {SaltValue = GetSalt()};
            text += hSalt.SaltValue;

            var bytes = new byte[text.Length * sizeof(char)];
            Buffer.BlockCopy(text.ToCharArray(), 0, bytes, 0, bytes.Length);
            var hashedBytes = Md5CryptoService.ComputeHash(bytes);
            hSalt.HashValue = hashedBytes.ToString();

            return hSalt;
        }

        /// <summary>
        /// Gets the salt.
        /// </summary>
        /// <returns></returns>
        private static string GetSalt()
        {
            var saltBytes = new byte[SaltSize];
            RngCryptoService.GetNonZeroBytes(saltBytes);
            return saltBytes.ToString();
        }
    }

    public static class Extensions
    {
        public static string ToString(this byte[] bytes)
        {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }

    /// <summary>
    /// Class to store hash and salt value
    /// </summary>
    public class HashSalt
    {
        public string HashValue { get; set; }
        public string SaltValue { get; set; }
    }
}
