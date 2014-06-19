using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SuperPutty.Data
{
    public class KeyboardShortcut
    {
        public string Name { get;set; }

        public Keys Key { get; set; }
        public Keys Modifiers { get; set; }

        public void Clear()
        {
            this.Key = Keys.None;
            this.Modifiers = Keys.None;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            if (this.Name != null){
                hash ^= this.Name.GetHashCode();
            }
            hash ^= this.Key.GetHashCode();
            hash ^= this.Modifiers.GetHashCode();

            return hash;
        }

        public override bool Equals(object thatObj)
        {
            KeyboardShortcut that = thatObj as KeyboardShortcut;
            return that != null &&
                this.Name == that.Name &&
                this.Key == that.Key &&
                this.Modifiers == that.Modifiers;
        }

        public string ShortcutString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (this.Modifiers != Keys.None)
                {
                    AppendIfSet(sb, this.Modifiers, Keys.Control, "Ctrl");
                    AppendIfSet(sb, this.Modifiers, Keys.Alt, "Alt");
                    AppendIfSet(sb, this.Modifiers, Keys.Shift, "Shift");
                }
                if (this.Key != Keys.None)
                {
                    sb.Append(this.Key);
                }
                return sb.ToString();
            }
        }

        public override string ToString()
        {
            return string.Format("[Shortcut name={0}, key={1}, modifiers={2}]", this.Name, this.Key, this.Modifiers);
        }

        #region Utils

        public static KeyboardShortcut FromKeys(Keys keys)
        {
            KeyboardShortcut ks = new KeyboardShortcut();

            // check for modifers and remove from val
            if (IsSet(keys, Keys.Control)) 
            {  
                ks.Modifiers |= Keys.Control;
                keys ^= Keys.Control;
            }
            if (IsSet(keys, Keys.Alt)) 
            { 
                ks.Modifiers |= Keys.Alt;
                keys ^= Keys.Alt;
            }
            if (IsSet(keys, Keys.Shift)) 
            { 
                ks.Modifiers |= Keys.Shift;
                keys ^= Keys.Shift;
            }

            // remaining should be the key
            ks.Key = keys;

            return ks;
        }

        static void AppendIfSet(StringBuilder sb, Keys modifers, Keys key, string keyText)
        {
            if (IsSet(modifers, key))
            {
                sb.Append(keyText).Append("+");
            }
        }

        static bool IsSet(Keys modifiers, Keys key)
        {
            return (modifiers & key) == key;
        }

        #endregion
    }
}
