using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SuperPutty
{
    public partial class TopForm : Form
    {
        public TopForm()
        {
            InitializeComponent();

            this.Opacity = SuperPuTTY.Settings.Opacity;
        }
    }
}
