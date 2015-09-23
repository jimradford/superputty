using System;
using System.Security.Cryptography;
using System.Text;

namespace SuperPutty.Utils
{
    public class DataProtection
    {
        //https://msdn.microsoft.com/en-us/library/system.security.cryptography.protecteddata(v=vs.90).aspx
        // Create byte array for additional entropy when using Protect method.
        static byte[] _aditionalEntropy = { 9, 8, 7, 6, 5 };
        public static String Protect(String text)
        {
            try
            {
                byte[] textByte = Encoding.Unicode.GetBytes(text);
                // Encrypt the data using DataProtectionScope.CurrentUser. The result can be decrypted
                //  only by the same current user.
                byte[] result = ProtectedData.Protect(textByte, _aditionalEntropy, DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(result);

            }
            catch (CryptographicException e)
            {
                Console.WriteLine("ERROR DataProtection.Protect .Data was not encrypted. An error occurred.");
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public static String Unprotect(String text)
        {
            try
            {
                byte[] textByte = Convert.FromBase64String(text);
                //Decrypt the data using DataProtectionScope.CurrentUser.
                byte[] result = ProtectedData.Unprotect(textByte, _aditionalEntropy, DataProtectionScope.CurrentUser);
                return Encoding.Unicode.GetString(result);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("ERROR DataProtection.Unprotect Data was not decrypted. An error occurred.");
                Console.WriteLine(e.ToString());
                return null;
            }
        }
    }

}