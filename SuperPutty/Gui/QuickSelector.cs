﻿using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using log4net;

namespace SuperPutty.Gui
{
    public partial class QuickSelector : TopForm
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(QuickSelector));
        private static char[] sanitizeChars = { '*', '[', ',' };
        private static char[] splitChars = { ' ', '.', '*' };

        public QuickSelector()
        {
            InitializeComponent();
        }

        void DoClose()
        {
            this.SelectedItem = null;
            this.DialogResult = DialogResult.Cancel;
        }

        void DoSelectItem()
        {
            DataGridViewSelectedRowCollection selectedRows = this.dataGridViewData.SelectedRows;
            if (selectedRows.Count == 1)
            {
                DataRowView row = (DataRowView) selectedRows[0].DataBoundItem;
                this.SelectedItem = (QuickSelectorData.ItemDataRow) row.Row;
                this.DialogResult = DialogResult.OK;
            }
        }

        void UpdateFilter()
        {
            this.DataView.RowFilter = String.IsNullOrEmpty(this.textBoxData.Text) ? String.Empty : FormatFilterString(this.textBoxData.Text);
            this.Text = string.Format("{0} [{1}]", this.Options.BaseText, this.DataView.Count);
        }

        string FormatFilterString(string text)
        {
            string filter = "";

            int i = 0;
            int tokenCount = cleanTokens(text.Split(splitChars)).Length;

            foreach (string token in cleanTokens(text.Split(splitChars)))
            {
                if (i > 0 && i < tokenCount)
                {
                    filter += " AND ";
                }

                filter += string.Format("([Name] LIKE '%{0}%' OR [Detail] LIKE '%{0}%')", tokenSanitize(token));

                i++;
            }

            return filter;
        }

        string[] cleanTokens(string[] tokens)
        {
            int i = tokens.Count(token => token.Length > 0);

            string[] result = new string[i];

            i = 0;
            foreach(string token in tokens)
            {
                if (token.Length > 0)
                {
                    result[i] = token;
                    i++;
                }
            }
            return result;
        }

        string tokenSanitize(string token)
        {
            return sanitizeChars.Aggregate(token, (current, sanitizeChar) => current.Replace(Convert.ToString(sanitizeChar), ""));
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Log.InfoFormat("Closed - DialogResult={0}, SelectedItem={1}", 
                this.DialogResult, this.SelectedItem != null ? this.SelectedItem.Name + ":" + this.SelectedItem.Detail : "");
        }

        private void textBoxData_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (this.DataView.Count > 0)
                    {
                        this.DoSelectItem();
                    }
                    break;
                case Keys.Escape:
                    this.DoClose();
                    e.Handled = true;
                    break;
                case Keys.Down:
                    // focus grid and move selection down by 1 row if possible
                    this.dataGridViewData.Focus();
                    if (this.dataGridViewData.SelectedRows[0].Index == 0)
                    {
                        if (this.dataGridViewData.Rows.Count > 1)
                        {
                            this.dataGridViewData.CurrentCell = this.dataGridViewData.Rows[1].Cells[0];
                        }
                    }
                    e.Handled = true;
                    break;
                case Keys.Back:
                    if (e.Control && this.textBoxData.SelectionStart == this.textBoxData.Text.Length)
                    {
                        // delete word
                        int idx = this.textBoxData.Text.LastIndexOf("/");
                        if (idx != -1)
                        {
                            this.textBoxData.Text = this.textBoxData.Text.Substring(0, idx);
                            this.textBoxData.SelectionStart = this.textBoxData.Text.Length;
                        }
                        else
                        {
                            this.textBoxData.Text = "";
                        }
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;
                default:
                    break;
            }
        }

        private void textBoxData_TextChanged(object sender, EventArgs e)
        {
            this.UpdateFilter();
        }

        private void dataGridViewData_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    this.DoSelectItem();
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    this.DoClose();
                    e.Handled = true;
                    break;
                case Keys.Up:
                    if (this.dataGridViewData.Rows[0].Selected)
                    {
                        this.textBoxData.Focus();
                        e.Handled = true;
                    }
                    break;
                default:
                    break;
            }
        }

        private void dataGridViewData_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            this.DoSelectItem();
        }

        private void dataGridViewData_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                QuickSelectorData.ItemDataRow row = 
                    (QuickSelectorData.ItemDataRow) ((DataRowView)this.dataGridViewData.Rows[e.RowIndex].DataBoundItem).Row;
                if (!row.IsTextColorNull())
                {
                    e.CellStyle.ForeColor = (Color) row.TextColor;
                }
            }
        }

        Brush highLighter = new SolidBrush(Color.FromArgb(120, 255, 255, 0));

        private void dataGridViewData_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (!this.Options.HighlightMatchingText) { return; }

            // draw default
            e.Paint(e.ClipBounds, DataGridViewPaintParts.All);

            String txt = this.textBoxData.Text;
            if (txt.Length > 0)
            {
                String display = (String) e.FormattedValue;
                
                int idx = display.IndexOf(txt);
                if (idx != -1)
                {
                    String skipText = display.Substring(0, idx);
                    SizeF match = e.Graphics.MeasureString(txt, e.CellStyle.Font);
                    SizeF skip = e.Graphics.MeasureString(skipText, e.CellStyle.Font);

                    // highlight matching text
                    Rectangle newRect = new Rectangle(
                        e.CellBounds.X + (int) skip.Width,
                        (int) (e.CellBounds.Y + (e.CellBounds.Height - match.Height) / 2), 
                        (int) match.Width - 3, 
                        (int) match.Height + 2);
                    e.Graphics.FillRectangle(highLighter, newRect);
                }
            }

            e.Handled = true;
        }

        public DialogResult ShowDialog(IWin32Window parent, QuickSelectorData data, QuickSelectorOptions options)
        {
            // bind data
            this.Options = options;
            this.DataView = new DataView(data.ItemData) {Sort = options.Sort};
            this.dataGridViewData.DataSource = this.DataView;

            // configure grid
            this.nameDataGridViewTextBoxColumn.Visible = this.Options.ShowNameColumn;
            this.detailDataGridViewTextBoxColumn.Visible = this.Options.ShowDetailColumn;
            if (this.Options.ShowDetailColumn && !this.Options.ShowNameColumn)
            {
                this.detailDataGridViewTextBoxColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

            // update title
            this.UpdateFilter();            
            return ShowDialog(parent);
        }

        DataView DataView { get; set; }
        public QuickSelectorOptions Options { get; private set; }
        public QuickSelectorData.ItemDataRow SelectedItem { get; private set; }
    }
    public class QuickSelectorOptions
    {
        public QuickSelectorOptions()
        {
            this.ShowDetailColumn = true;
            this.BaseText = "Select Item";
        }
        public bool ShowNameColumn { get; set; }
        public bool ShowDetailColumn { get; set; }
        public string BaseText { get; set; }
        public string Sort { get; set; }
        public bool HighlightMatchingText { get; set; }
    }
}
