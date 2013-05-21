using System;
using System.ComponentModel;
using SuperPutty.Gui;
using System.Threading;

namespace SuperPutty.Scp
{
    /// <summary>
    /// Adapter class over the IBrowserModel to facilitate GUI binding
    /// </summary>
    public class BrowserViewModel : BaseViewModel, IBrowserViewModel
    {
        string currentPath;
        string status;
        BrowserState browserState;

        public BrowserViewModel()
        {
            this.currentPath = null;
            this.status = String.Empty;
            this.browserState = BrowserState.Ready;
            this.Files = new BindingList<BrowserFileInfo>();
            this.Context = SynchronizationContext.Current;
        }

        public string CurrentPath
        {
            get { return this.currentPath; }
            set { SetField(ref this.currentPath, value, () => this.CurrentPath); }
        }

        public string Status
        {
            get { return this.status; }
            set { SetField(ref this.status, value, () => this.Status); }
        }

        public BrowserState BrowserState
        {
            get { return this.browserState; }
            set { SetField(ref this.browserState, value, () => this.BrowserState); }
        }

        public BindingList<BrowserFileInfo> Files { get; private set; }
    }

}
