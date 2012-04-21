using log4net;
using Microsoft.Win32;
using System.IO;

namespace SuperPutty.Properties {
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    internal sealed partial class Settings {

        private static readonly ILog Log = LogManager.GetLogger(typeof(Settings));

        public Settings() {
            this.SettingChanging += this.SettingChangingEventHandler;
            this.SettingsSaving += this.SettingsSavingEventHandler;
        }

        protected override void OnSettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
        {
            Log.InfoFormat("Settings Loaded");
            base.OnSettingsLoaded(sender, e);
            //Log.Info("Settings Folder=" + this.SettingsFolder);

            if (SuperPuTTY.IsFirstRun)
            {
                Log.Info("PuttyExe empty in settings.  Attempting import from registry");
                LoadFromRegistry();
            }
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // Add code to handle the SettingChangingEvent event here.

        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Add code to handle the SettingsSaving event here.
            Log.InfoFormat("Settings Saved");
        }

        /// <summary>
        /// Load old settings from registry into Settings
        /// </summary>
        private void LoadFromRegistry()
        {
            // Get Registry Entry for Putty Exe
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Jim Radford\SuperPuTTY\Settings");
            if (key != null)
            {
                string puttyExe = key.GetValue("PuTTYExe", "").ToString();
                if (File.Exists(puttyExe))
                {
                    Log.Debug("Imported PuttyExe from Registry");
                    PuttyExe = puttyExe;
                }

                string pscpExe = key.GetValue("PscpExe", "").ToString();
                if (File.Exists(pscpExe))
                {
                    Log.Debug("Imported PscpExe from Registry");
                    PscpExe = pscpExe;
                }
            }
        }
    }
}
