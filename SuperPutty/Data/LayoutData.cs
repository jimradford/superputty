using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SuperPutty.Data
{
    public class LayoutData
    {
        public const string AutoRestore = "<Auto Restore>";
        public const string AutoRestoreLayoutFileName = "AutoRestoreLayout.XML";

        public LayoutData(string filePath)
        {
            this.FilePath = filePath;
            this.Name = Path.GetFileNameWithoutExtension(filePath);
        }

        public string Name { get; set; }
        public string FilePath { get; set; }

        public bool IsDefault {
            get { return this.Name == SuperPuTTY.Settings.DefaultLayoutName;  } 
        }

        public override string ToString()
        {
            return IsDefault ? String.Format("{0} (default)", this.Name) : this.Name;
        }
    }

    public class LayoutChangedEventArgs : EventArgs
    {
        public LayoutData New { get; set; }
        public LayoutData Old { get; set; }
        public bool IsNewLayoutAlreadyActive { get; set; }
    }
}
