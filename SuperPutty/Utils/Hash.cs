using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SuperPutty.Utils
{
    class Hash
    {        
        private static byte[] GetHash(string text)
        {
            HashAlgorithm algorithm = SHA512.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(text));
        }

        public static string GetHashString(string text)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byteHash = GetHash(text);
            foreach (byte b in byteHash)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }
    }
}
