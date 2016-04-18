using System;
using System.Linq;
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
            this.IsControlSet = IsSet(this.Shortcut.Modifiers, Keys.Control);
            this.IsAltSet = IsSet(this.Shortcut.Modifiers, Keys.Alt);
            this.IsShiftSet = IsSet(this.Shortcut.Modifiers, Keys.Shift);

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
            return modifiers.Any(modifier => (keys & modifier) == modifier);
        }

        public bool IsControlSet;
        public bool IsAltSet;
        public bool IsShiftSet;

        public KeyboardShortcut Shortcut { get; private set; }
        public Form Form { get; private set; }
        public int Id { get; private set; }

        public int Modifiers { get; private set; }
        public Keys Key { get; private set; }
    }

}
