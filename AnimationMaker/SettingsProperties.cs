using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace AnimationMaker
{
    class SettingsProperties
    {
        public static String ImageFolder
        {
            get
            {
                string FolderString = "";
                RegistryKey SettingsKey = Registry.CurrentUser.CreateSubKey("K15 AnimationMaker");
                FolderString = (string)SettingsKey.GetValue("ImageFolder");
                SettingsKey.Close();

                return FolderString;
            }
            set
            {
                RegistryKey SettingsKey = Registry.CurrentUser.CreateSubKey("K15 AnimationMaker");
                SettingsKey.SetValue("ImageFolder", value);
                SettingsKey.Close();
            }
        }
    }
}
