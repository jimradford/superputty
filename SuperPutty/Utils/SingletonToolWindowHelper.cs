using System;
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
        public delegate void InstanceChangedHandler(T Instance);

        public event InstanceChangedHandler InstanceChanged;

        public SingletonToolWindowHelper(string name, DockPanel dockPanel) : this(name, dockPanel, null, null) {}

        public SingletonToolWindowHelper(string name, DockPanel dockPanel, Object InitializerResource, WindowInitializer initializer)
        {
            this.Name = name;
            this.DockPanel = dockPanel;
            this.Initializer = initializer;
            this.InitializerResource = InitializerResource;
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

        public void ShowWindow(DockPane pane, IDockContent PreviousContent)
        {
            if (this.Instance == null)
            {
                Initialize();
            }
            
            Instance.Show(pane, PreviousContent);
            SuperPuTTY.ReportStatus("Showing " + this.Name);
        }

        public bool IsVisibleAsToolWindow => this.Instance != null && this.Instance.DockHandler.Pane != null && !this.Instance.DockHandler.Pane.IsAutoHide;

        public T Initialize()
        {
            this.Instance = this.Initializer == null ? Activator.CreateInstance<T>() : this.Initializer(this);

            this.Instance.FormClosed += new FormClosedEventHandler(Instance_FormClosed);
            if (InstanceChanged != null)
            {
                InstanceChanged(this.Instance);
            }
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

        public bool IsVisible => this.Instance != null && this.Instance.Visible;


        public string Name { get; private set; }
        public DockPanel DockPanel { get; private set; }
        public WindowInitializer Initializer { get; private set; }
        public Object InitializerResource { get; private set; }
        public T Instance { get; private set; }
    }
}
