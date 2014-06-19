using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SuperPutty.Utils
{
    /// <summary>
    /// Helper class to track single instance tool windows (sessions, log viewer, layouts)
    /// </summary>
    public class SingletonToolWindowHelper<T> where T : ToolWindow
    {
        public delegate T WindowInitializer(SingletonToolWindowHelper<T> helper);

        public SingletonToolWindowHelper(string name, DockPanel dockPanel) : this(name, dockPanel, null) {}

        public SingletonToolWindowHelper(string name, DockPanel dockPanel, WindowInitializer initializer)
        {
            this.Name = name;
            this.DockPanel = dockPanel;
            this.Initializer = initializer;
        }

        public void ShowWindow(DockState dockState)
        {
            if (this.Instance == null)
            {
                Initialize();
                this.Instance.Show(DockPanel, dockState);
                SuperPuTTY.ReportStatus("Showing " + this.Name);
            }
            else
            {
                this.Instance.Show(DockPanel);
                SuperPuTTY.ReportStatus("Bringing {0} to Front", this.Name);
            }
        }

        public void ShowWindow(DockPane pane, DockAlignment dockAlign, double proportion)
        {
            if (this.Instance == null)
            {
                Initialize();
                this.Instance.Show(pane, dockAlign, proportion);
                SuperPuTTY.ReportStatus("Showing " + this.Name);
            }
            else
            {
                this.Instance.Show(DockPanel);
                SuperPuTTY.ReportStatus("Bringing {0} to Front", this.Name);
            }
        }

        public bool IsVisibleAsToolWindow
        {
            get
            {
                return this.Instance != null && this.Instance.DockHandler.Pane != null && !this.Instance.DockHandler.Pane.IsAutoHide;
            }
        }

        public T Initialize()
        {
            if (this.Initializer == null)
            {
                // assume defautl ctor ok
                this.Instance = Activator.CreateInstance<T>();
            }
            else
            {
                // some kind of factor method throw in
                this.Instance = this.Initializer(this);
            }
            this.Instance.FormClosed += new FormClosedEventHandler(Instance_FormClosed);
            return Instance;
        }

        void Instance_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Instance = null;
            SuperPuTTY.ReportStatus("Closed {0}", this.Name);
        }

        public void Hide()
        {
            if (this.Instance != null)
            {
                this.Instance.Hide();
            }
        }

        public void Restore()
        {
            if (this.Instance != null)
            {
                this.Instance.Show(this.DockPanel);
            }
        }

        public bool IsVisible
        {
            get { return this.Instance != null && this.Instance.Visible; }
        }


        public string Name { get; private set; }
        public DockPanel DockPanel { get; private set; }
        public WindowInitializer Initializer { get; private set; }
        public T Instance { get; private set; }
    }
}
