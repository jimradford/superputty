using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Windows.Forms;
using SuperPutty.Gui;
using SuperPutty.Data;

namespace SuperPuttyUnitTests
{
    [TestFixture]
    public class KeyboardShortcutsTets
    {
        [Test]
        public void KeysToStringTest()
        {
            String s = Keys.None.ToString();
            Assert.AreEqual(s, "None");

            Keys k = Keys.Control;
            Assert.AreEqual("Control", k.ToString());
            k |= Keys.M;
            Assert.AreEqual("M, Control", k.ToString());

            k |= Keys.Shift;
            Assert.AreEqual("M, Shift, Control", k.ToString());

            Keys k2 = (Keys) Enum.Parse(typeof(Keys), k.ToString());
            Assert.AreEqual(k, k2);
        }

        [Test]
        public void ToStringTest()
        {
            KeyboardShortcut ks = new KeyboardShortcut();

            Assert.AreEqual("", ks.ShortcutString);

            ks.Key = Keys.F11;
            Assert.AreEqual("F11", ks.ShortcutString);

            ks.Key = Keys.PageUp;
            ks.Modifiers = Keys.Control;
            Assert.AreEqual("Ctrl+PageUp", ks.ShortcutString);

            ks.Modifiers |= Keys.Shift;
            Assert.AreEqual("Ctrl+Shift+PageUp", ks.ShortcutString);

        }

        [TestView]
        public void DialogBasicTest()
        {
            KeyboardShortcutEditor form = new KeyboardShortcutEditor();
            form.ShowDialog(null, new KeyboardShortcut { Name = "test", Key = Keys.A, Modifiers = Keys.Control });
        }

    }
}
