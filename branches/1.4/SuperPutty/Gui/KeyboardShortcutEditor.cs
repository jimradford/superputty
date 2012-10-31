using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SuperPutty.Data;

namespace SuperPutty.Gui
{
    public partial class KeyboardShortcutEditor : Form
    {
        KeyboardShortcut KeyboardShortcut { get; set; }

        public KeyboardShortcutEditor()
        {
            InitializeComponent();
            this.KeyboardShortcut = new KeyboardShortcut();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.BeginInvoke(new Action(DoFocus));
        }

        void DoFocus()
        {
            this.textBoxKeys.Focus();
        }

        /// <summary>
        /// Show dialog to edit shortcut
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="shortcut"></param>
        /// <returns>return null if canceled</returns>
        public DialogResult ShowDialog(IWin32Window parent, KeyboardShortcut shortcut)
        {
            // init values
            this.Text = string.Format("Edit Shortcut - {0}", shortcut.Name);
            this.KeyboardShortcut.Key = shortcut.Key;
            this.KeyboardShortcut.Modifiers = shortcut.Modifiers;
            this.textBoxKeys.Text = this.KeyboardShortcut.ShortcutString;

            // show dialog
            DialogResult result = ShowDialog(parent);
            if (result == DialogResult.OK)
            {
                // update values
                shortcut.Key = this.KeyboardShortcut.Key;
                shortcut.Modifiers = this.KeyboardShortcut.Modifiers;
            }

            return result;
        }

        private void textBoxKeys_KeyDown(object sender, KeyEventArgs e)
        {
            this.KeyboardShortcut.Key = e.KeyCode;
            this.KeyboardShortcut.Modifiers = e.Modifiers;

            this.textBoxKeys.Text = this.KeyboardShortcut.ShortcutString;
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.KeyboardShortcut.Key = Keys.None;
            this.KeyboardShortcut.Modifiers = Keys.None;
            this.textBoxKeys.Text = this.KeyboardShortcut.ShortcutString;
        }


    }
}
