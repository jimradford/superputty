using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Web;

namespace SuperPutty.Data
{
    public class PuttyDataHelper
    {
        public static List<string> GetSessionNames()
        {
            List<string> names = new List<string>();

            RegistryKey key = SuperPuTTY.IsKiTTY
                ? Registry.CurrentUser.OpenSubKey(@"Software\9bis.com\KiTTY\Sessions")
                : Registry.CurrentUser.OpenSubKey(@"Software\SimonTatham\PuTTY\Sessions");
            if (key != null)
            {
                string[] savedSessionNames = key.GetSubKeyNames();
                foreach (string rawSession in savedSessionNames)
                {
                    names.Add(HttpUtility.UrlDecode(rawSession));
                }
            }

            return names;
        }
    }
}
