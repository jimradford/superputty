using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperPutty.Utils
{
    class SingletonSessionPasswordManager
    {
        private static SingletonSessionPasswordManager instance;
        private static String masterPassword = "";       


        private SingletonSessionPasswordManager() {
            refreshMasterPassword();
        }

        /// <summary>
        /// refresh the masterPassword stored in memory from settings
        /// </summary>
        public void refreshMasterPassword (){ 
            masterPassword = DataProtection.Unprotect(SuperPuTTY.Settings.PasswordImportExport);
        }

        /// <summary>
        /// get the MasterPassword stored in memory
        /// </summary>
        /// <returns>MasterPassword</returns>
        public String getMasterPassword() {
            return masterPassword;
        }

        /// <summary>
        /// set the master password in memory and in settings
        /// </summary>
        /// <param name="pw">new password</param>
        public void setMasterPassword(String pw)
        {
            if (String.IsNullOrEmpty(pw))
            {
                masterPassword = "";
                SuperPuTTY.Settings.PasswordImportExport = "";
            }else {
                masterPassword = Hash.GetHashString(pw);
                SuperPuTTY.Settings.PasswordImportExport = DataProtection.Protect(masterPassword);
            }
            
        }

        public static SingletonSessionPasswordManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SingletonSessionPasswordManager();
                }
                return instance;
            }
        }
    }
}
