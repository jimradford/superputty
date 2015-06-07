using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace SuperPutty.Utils
{
    public class CryptoAES
    {
        private static readonly byte[] Salt = new byte[] { 10, 20, 30, 40, 50, 60, 70, 80 };
        const int Iterations = 300;

        public static string EncryptString(string text, string textKey)
        {
            byte[] textByte = Encoding.Unicode.GetBytes(text);
            MemoryStream ms;
            CryptoStream cs;
            Rijndael rj = Rijndael.Create();
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(textKey, Salt, Iterations);
            rj.Key = key.GetBytes(32);
            rj.IV = key.GetBytes(16);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, rj.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(textByte, 0, textByte.Length);
            cs.Close();
            return Convert.ToBase64String(ms.ToArray());
        }

        
        public static string DecryptString(string text, string textKey)
        {
            byte[] textByte = Convert.FromBase64String(text); 
            MemoryStream ms;
            CryptoStream cs;
            Rijndael rj = Rijndael.Create();
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(textKey, Salt, Iterations);
            rj.Key = key.GetBytes(32);
            rj.IV = key.GetBytes(16);
            ms = new MemoryStream();
            cs = new CryptoStream(ms, rj.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(textByte, 0, textByte.Length);
            cs.Close();
            return Encoding.Unicode.GetString(ms.ToArray());
        }


    }
}
