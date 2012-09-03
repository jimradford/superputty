using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net;

namespace SuperPutty.Gui
{
    public partial class QuickSelector : Form
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(QuickSelector));

        public QuickSelector()
        {
            InitializeComponent();
        }

        private void comboBoxData_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (this.comboBoxData.DroppedDown)
                    {
                        // close drop down on 1st esc
                        this.comboBoxData.DroppedDown = false;
                    }
                    else
                    {
                        // 2nd, closes form
                        this.DoClose();
                    }
                    break;
                case Keys.Enter:
                    if (this.comboBoxData.DroppedDown)
                    {
                        if (this.comboBoxData.Items.Count == 1)
                        {
                            // only one
                            this.SelectedItem = (QuickSelectorData.ItemDataRow)this.comboBoxData.Items[0];
                        }
                        else
                        {
                            //this.comboBoxData
                        }
                    }
                    this.DoSelectItem();
                    break;
                default:
                    if (!this.comboBoxData.DroppedDown)
                    {
                        this.comboBoxData.DroppedDown = true;
                    }
                    e.Handled = false;
                    break;
            }
        }

        void DoClose()
        {
            this.SelectedItem = null;
            this.DialogResult = DialogResult.Cancel;
        }

        void DoSelectItem()
        {
            if (this.comboBoxData.DroppedDown)
            {
                // drop down open
            }
            this.SelectedItem = (QuickSelectorData.ItemDataRow)this.comboBoxData.SelectedItem;
            this.DialogResult = DialogResult.OK;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Log.InfoFormat("Closed - DialogResult={0}, SelectedItem={1}", this.DialogResult, this.SelectedItem);
        }

        public DialogResult ShowDialog(IWin32Window parent, QuickSelectorData data, string baseText, string sort)
        {
            this.DataView = new DataView(data.ItemData);
            this.DataView.Sort = sort;
            this.BaseText = baseText;

            this.comboBoxData.DataSource = this.DataView;

            return ShowDialog(parent);
        }

        DataView DataView { get; set; }
        public string BaseText { get; private set; }
        public QuickSelectorData.ItemDataRow SelectedItem { get; private set; }

    }
}
