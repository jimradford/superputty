using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SuperPutty.Data;

namespace SuperPutty.Utils
{
    public class GlobalHotkey : IDisposable
    {
        public GlobalHotkey(Form form, KeyboardShortcut shortcut)
        {
            this.Form = form;
            this.Shortcut = shortcut;

            // convert the Keys to modifiers
            this.Modifiers = NativeMethods.HotKeysConstants.NOMOD;
            if (IsControlSet)
            {
                this.Modifiers += NativeMethods.HotKeysConstants.CTRL;
            }
            if (IsAltSet)
            {
                this.Modifiers += NativeMethods.HotKeysConstants.ALT;
            }
            if (IsShiftSet)
            {
                this.Modifiers += NativeMethods.HotKeysConstants.SHIFT;
            }

            // make uid
            this.Id = this.Shortcut.GetHashCode() ^ this.Form.Handle.ToInt32();

            this.Register();
        }

        public bool Register()
        {
            return NativeMethods.RegisterHotKey((IntPtr) this.Form.Handle, this.Id, this.Modifiers, (int) this.Shortcut.Key);
        }

        public void Dispose()
        {
            NativeMethods.UnregisterHotKey(this.Form.Handle, this.Id);
        }

        private static bool IsSet(Keys keys, params Keys[] modifiers)
        {
            foreach(Keys modifier in modifiers)
            {
                if ((keys & modifier) == modifier)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsControlSet { get { return IsSet(this.Shortcut.Modifiers, Keys.Control); } }
        public bool IsAltSet { get { return IsSet(this.Shortcut.Modifiers, Keys.Alt); } }
        public bool IsShiftSet { get { return IsSet(this.Shortcut.Modifiers, Keys.Shift); } }

        public KeyboardShortcut Shortcut { get; private set; }
        public Form Form { get; private set; }
        public int Id { get; private set; }

        public int Modifiers { get; private set; }
        public Keys Key { get; private set; }
    }

}
