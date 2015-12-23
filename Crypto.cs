using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
 
namespace Tester
{
    /*
     * Coded by Sam
     */
 
    /// <summary>
    /// Crypto class
    /// </summary>
    public static class Crypto
    {
        /// <summary>
        /// Algorithm ways
        /// </summary>
        public enum Algorithm
        {
            Encrypt,
            Decrypt
        }
 
        #region EasyMethods
        public static string getMD5(this string data, string encoding="ASCII")
        {
            return getHash(Encoding.GetEncoding(encoding).GetBytes(data), new MD5CryptoServiceProvider());
        }
 
        public static string getSHA1(this string data, string encoding = "ASCII")
        {
            return getHash(Encoding.GetEncoding(encoding).GetBytes(data), new SHA1CryptoServiceProvider());
        }
 
        public static string getSHA256(this string data, string encoding = "ASCII")
        {
            return getHash(Encoding.GetEncoding(encoding).GetBytes(data), new SHA256CryptoServiceProvider());
        }
 
        public static string getSHA384(this string data, string encoding = "ASCII")
        {
            return getHash(Encoding.GetEncoding(encoding).GetBytes(data), new SHA384CryptoServiceProvider());
        }
 
        public static string getSHA512(this string data, string encoding = "ASCII")
        {
            return getHash(Encoding.GetEncoding(encoding).GetBytes(data), new SHA512CryptoServiceProvider());
        }
 
        public static byte[] getDES(this byte[] data, string key, Algorithm todo, byte[] rgbIV = null)
        {
            return getSymAlgorithm(data, key, todo, new DESCryptoServiceProvider(), 8, rgbIV);
        }
 
        public static byte[] getRC2(this byte[] data, string key, Algorithm todo, byte[] rgbIV = null)
        {
            return getSymAlgorithm(data, key, todo, new RC2CryptoServiceProvider(), 16, rgbIV);
        }
 
        public static byte[] getTripleDES(this byte[] data, string key, Algorithm todo, byte[] rgbIV = null)
        {
            return getSymAlgorithm(data, key, todo, new TripleDESCryptoServiceProvider(), 16, rgbIV);
        }
 
        public static byte[] getRijndael(this byte[] data, string key, Algorithm todo, byte[] rgbIV = null)
        {
            return getSymAlgorithm(data, key, todo, new RijndaelManaged(), 16, rgbIV);
        }
 
        public static byte[] getAes(this byte[] data, string key, Algorithm todo, byte[] rgbIV = null)
        {
            return getSymAlgorithm(data, key, todo, new AesCryptoServiceProvider(), 16, rgbIV);
        }
        #endregion
 
        /// <summary>
        /// Get a hash of the data
        /// </summary>
        /// <param name="data">Data for the hash</param>
        /// <param name="h">Hash crypto service</param>
        /// <returns>Return a hash(by a crypto service) of the data</returns>
        public static string getHash(this byte[] data, HashAlgorithm h)
        {
            return String.Join("", h.ComputeHash(data, 0, data.Length).Select(x => x.ToString("X2").Replace("-", "").ToLower()));
        }
 
        /// <summary>
        /// Get the result of a symmetric algorithm (yep a bit minimal because i'm lazy)
        /// </summary>
        /// <param name="data">Data to encrypt</param>
        /// <param name="key">Key to use for encrypt</param>
        /// <param name="todo">Algorithm way</param>
        /// <param name="h">Crypto service or managed class of the algorithm</param>
        /// <param name="bytes">Bytes to get for the pseudorandom identifier</param>
        /// <param name="rgbIV">Vector of the algorithm</param>
        /// <returns>Encrypted or decrypted data by a symmetric algorithm</returns>
        public static byte[] getSymAlgorithm(this byte[] data, string key, Algorithm todo, SymmetricAlgorithm h, int bytes, byte[] rgbIV = null)
        {
            try
            {
                rgbIV = rgbIV ?? new byte[h.BlockSize / 8];
                byte[] k = new Rfc2898DeriveBytes(key, new byte[8]).GetBytes(bytes);
                return todo == Algorithm.Encrypt ? h.CreateEncryptor(k, rgbIV).TransformFinalBlock(data, 0, data.Length) : h.CreateDecryptor(k, rgbIV).TransformFinalBlock(data, 0, data.Length);
            }
            catch (CryptographicException) { return null; }
        }
    }
}
