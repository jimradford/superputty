using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Windows.Forms;
using SuperPutty.Gui;
using SuperPutty.Data;
using System.Drawing;

namespace SuperPuttyUnitTests
{
    [TestFixture]
    public class QuickSelectorTests
    {
        static QuickSelectorTests()
        {
            log4net.Config.BasicConfigurator.Configure();
        }


        [Test]
        public void Test()
        {
            List<SessionData> sessions = SessionData.LoadSessionsFromFile("c:/Users/beau/SuperPuTTY/sessions.xml");
            QuickSelectorData data = new QuickSelectorData();

            foreach (SessionData sd in sessions)
            {
                data.ItemData.AddItemDataRow(
                    sd.SessionName, 
                    sd.SessionId, 
                    sd.Proto == ConnectionProtocol.Cygterm || sd.Proto == ConnectionProtocol.Mintty ? Color.Blue : Color.Black, null);
            }

            QuickSelectorOptions opt = new QuickSelectorOptions();
            opt.Sort = data.ItemData.DetailColumn.ColumnName;
            opt.BaseText = "Open Session";

            QuickSelector d = new QuickSelector();
            d.ShowDialog(null, data, opt);
        }


    }
}
