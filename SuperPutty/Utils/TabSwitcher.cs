using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;
using log4net;

namespace SuperPutty.Utils
{
    #region TabSwitcher
    public class TabSwitcher : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TabSwitcher));

        public TabSwitcher(DockPanel dockPanel)
        {
            this.Documents = new List<ToolWindow>();

            this.DockPanel = dockPanel;
            this.DockPanel.ContentAdded += DockPanel_ContentAdded;
            this.DockPanel.ContentRemoved += DockPanel_ContentRemoved;
        }

        public ITabSwitchStrategy TabSwitchStrategy
        {
            get { return this.tabSwitchStrategy; }
            set
            {
                if (this.tabSwitchStrategy != value)
                {
                    // clean up
                    if (this.tabSwitchStrategy != null)
                    {
                        this.tabSwitchStrategy.Dispose();
                    }

                    // set and init new one
                    this.tabSwitchStrategy = value;
                    if (value != null)
                    {
                        this.tabSwitchStrategy.Initialize(this.DockPanel);
                        foreach (IDockContent doc in this.DockPanel.Documents)
                        {
                            this.AddDocument((ToolWindow)doc);
                        }
                        this.CurrentDocument = this.CurrentDocument;
                    }
                }
            }
        }

        public ToolWindow CurrentDocument
        {
            get { return this.currentDocument; }
            set
            {
                this.currentDocument = value;
                this.TabSwitchStrategy.SetCurrentTab(value);
            }
        }

        void DockPanel_ContentAdded(object sender, DockContentEventArgs e)
        {
            this.DockPanel.BeginInvoke(new Action(
                delegate
                {
                    if (e.Content.DockHandler.DockState == DockState.Document)
                    {
                        this.AddDocument((ToolWindow)e.Content);
                    }
                }));
        }

        void DockPanel_ContentRemoved(object sender, DockContentEventArgs e)
        {
            this.RemoveDocument((ToolWindow)e.Content);
        }

        void AddDocument(ToolWindow tab)
        {
            this.TabSwitchStrategy.AddTab(tab);
        }

        void RemoveDocument(ToolWindow tab)
        {
            this.TabSwitchStrategy.RemoveTab(tab);
        }

        public void MoveToNextDocument()
        {
            this.TabSwitchStrategy.MoveToNextTab();
        }

        public void MoveToPrevDocument()
        {
            this.TabSwitchStrategy.MoveToPrevTab();
        }


        void LogDocs(string x)
        {
            Log.InfoFormat("===================== {0}", x);
            foreach (ToolWindow doc in this.Documents)
            {
                Log.InfoFormat("{0} {1}", doc, doc == this.ActiveDocument ? "##" : "");
            }
        }

        public void Dispose()
        {
            this.DockPanel.ContentAdded -= DockPanel_ContentAdded;
            this.DockPanel.ContentRemoved -= DockPanel_ContentRemoved;
        }

        public bool MRU { get; set; }
        public ToolWindow ActiveDocument { get { return (ToolWindow)this.DockPanel.ActiveDocument; } }
        public DockPanel DockPanel { get; private set; }
        public IList<ToolWindow> Documents { get; private set; }

        ITabSwitchStrategy tabSwitchStrategy;
        ToolWindow currentDocument;
    } 
    #endregion

    #region ITabSwitchStrategy
    public interface ITabSwitchStrategy : IDisposable
    {
        void Initialize(DockPanel panel);
        void AddTab(ToolWindow tab);
        void RemoveTab(ToolWindow tab);
        void SetCurrentTab(ToolWindow window);

        void MoveToNextTab();
        void MoveToPrevTab();
    }
    #endregion

    #region OrderedTabSwitchStrategy 
    public class OrderedTabSwitchStrategy : ITabSwitchStrategy
    {
        public void Initialize(DockPanel panel) 
        {
            this.DockPanel = panel;
        }

        public void AddTab(ToolWindow tab) {}
        public void RemoveTab(ToolWindow tab) {}

        public void MoveToNextTab()
        {
            ToolWindow winNext = (ToolWindow)this.DockPanel.ActiveDocument;
            List<IDockContent> docs = new List<IDockContent>(this.DockPanel.DocumentsToArray());
            int idx = docs.IndexOf(this.DockPanel.ActiveDocument);
            if (idx != -1)
            {
                winNext = (ToolWindow)docs[idx == docs.Count - 1 ? 0 : idx + 1];
                winNext.Activate();
            }
        }

        public void MoveToPrevTab()
        {
            ToolWindow winPrev = (ToolWindow) this.DockPanel.ActiveDocument;
            List<IDockContent> docs = new List<IDockContent>(this.DockPanel.DocumentsToArray());
            int idx = docs.IndexOf(this.DockPanel.ActiveDocument);
            if (idx != -1)
            {
                winPrev = (ToolWindow) docs[idx == 0 ? docs.Count - 1 : idx - 1];
                winPrev.Activate();
            }
        }

        public void SetCurrentTab(ToolWindow window) {}
        public void Dispose() {}

        DockPanel DockPanel { get; set; }
    }
    #endregion

    #region MRUTabSwitchStrategy
    public class MRUTabSwitchStrategy : ITabSwitchStrategy
    {
        public void Initialize(DockPanel panel)
        {
            this.DockPanel = panel;
        }

        public void AddTab(ToolWindow newTab)
        {
            if (!nodes.ContainsKey(newTab))
            {
                // Insert this panel into the list used for Ctrl-Tab handling.

                // store node
                TabNode node = new TabNode { Window = newTab };
                nodes.Add(newTab, node);

                if (this.CurrentTab == null)
                {
                    // First panel to be created
                    node.Prev = node;
                    node.Next = node;
                    this.CurrentTab = node;
                } 
                else
                {
                    // Other panels exist. Tie ourselves into list ahead of current panel.
                    node.Prev = this.CurrentTab;
                    node.Next = this.CurrentTab.Next;
                    this.CurrentTab.Next = node;
                    node.Next.Prev = node;

                    // We are now the current panel
                    this.CurrentTab = node;
                }
            }
        }

        public void RemoveTab(ToolWindow oldTab)
        {
            TabNode node;
            if (this.nodes.TryGetValue(oldTab, out node))
            {
                // remove the tab
                this.nodes.Remove(oldTab);

                // remove the node from the circular list
                if (this.CurrentTab == node && node.Next == node && node.Prev == node)
                {
                    this.CurrentTab = null;
                }
                else
                {
                    node.Prev.Next = node.Next;
                    node.Next.Prev = node.Prev;
                    this.CurrentTab = node.Prev;
                }
            }
        }

        public void MoveToNextTab()
        {
            if (this.CurrentTab != null)
            {
                this.CurrentTab.Next.Window.Activate();
                //this.SetCurrentTab(this.CurrentTab.Next.Window);
            }
        }

        public void MoveToPrevTab()
        {
            if (this.CurrentTab != null)
            {
                this.CurrentTab.Prev.Window.Activate();
                //this.SetCurrentTab(this.CurrentTab.Prev.Window);
            }
        }

        public void SetCurrentTab(ToolWindow window)
        {
            TabNode node;
            if (window != null && this.nodes.TryGetValue(window, out node))
            {
                if (this.CurrentTab == node) { return; }

                // Remove ourselves from our position in chain
                node.Prev.Next = node.Next;
                node.Next.Prev = node.Prev;

                node.Prev = this.CurrentTab;
                node.Next = this.CurrentTab.Next;
                this.CurrentTab.Next = node;
                node.Next.Prev = node;

                this.CurrentTab = node;
            }
        }

        public void Dispose() { }

        DockPanel DockPanel { get; set; }
        TabNode CurrentTab { get; set; }

        private IDictionary<ToolWindow, TabNode> nodes = new Dictionary<ToolWindow, TabNode>();

        public class TabNode
        {
            public ToolWindow Window { get; set; }
            public TabNode Next { get; set; }
            public TabNode Prev { get; set; }
        }
    }
    #endregion


    #region ITabSwitcher
    public interface ITabSwitcher : IDisposable
    {
        event EventHandler<EventArgs> CurrentTabChanged;
        ToolWindow CurrentTab { get; set; }

        void Initialize(DockPanel dockPanel);
        void AddTab(ToolWindow newTab);
        void RemoveTab(ToolWindow oldTab);
        void MoveToNextTab();
        void MoveToPrevTab();
    } 
    #endregion

    #region AbstractTabSwitcher
    public abstract class AbstractTabSwitcher : ITabSwitcher
    {
        public event EventHandler<EventArgs> CurrentTabChanged;

        public abstract void AddTab(ToolWindow newTab);
        public abstract void RemoveTab(ToolWindow oldTab);
        public abstract void MoveToNextTab();
        public abstract void MoveToPrevTab();
        public abstract void Dispose();

        public virtual void Initialize(DockPanel dockPanel)
        {
            this.DockPanel = dockPanel;
            this.DockPanel.ContentAdded += DockPanel_ContentAdded;
            this.DockPanel.ContentRemoved += DockPanel_ContentRemoved;

            foreach (IDockContent doc in this.DockPanel.Documents)
            {
                this.AddTab((ToolWindow) doc);
            }
        }

        void DockPanel_ContentAdded(object sender, DockContentEventArgs e)
        {
            this.DockPanel.BeginInvoke(new Action(
                delegate
                {
                    if (e.Content.DockHandler.DockState == DockState.Document)
                    {
                        this.AddTab((ToolWindow)e.Content);
                    }
                }));
        }

        void DockPanel_ContentRemoved(object sender, DockContentEventArgs e)
        {
            this.RemoveTab((ToolWindow)e.Content);
        }

        public ToolWindow CurrentTab
        {
            get { return this.currentTab; }
            set
            {
                if (this.currentTab != value)
                {
                    this.currentTab = value;
                    this.OnCurrentTabChanged();
                }
            }
        }

        protected void OnCurrentTabChanged()
        {
            if (this.CurrentTabChanged != null)
            {
                this.CurrentTabChanged(this, EventArgs.Empty);
            }
        }

        protected DockPanel DockPanel { get; set; }
        protected ToolWindow currentTab;
    } 
    #endregion

    public class MRUTabSwitcher : AbstractTabSwitcher
    {
        public override void AddTab(ToolWindow newTab)
        {
            if (!nodes.ContainsKey(newTab))
            {
                // store node
                TabNode node = new TabNode { Window = newTab };
                nodes.Add(newTab, node);

                if (this.CurrentTab != null)
                {
                    TabNode nodeCurrent;
                    if (nodes.TryGetValue(this.CurrentTab, out nodeCurrent))
                    {
                        // attach the node
                        node.Prev = nodeCurrent;
                        node.Next = nodeCurrent.Next;
                        nodeCurrent.Next = node;
                        node.Next.Prev = node;
                    }
                }
                else
                {
                    // first tab
                    node.Prev = node;
                    node.Next = node;
                }
            }
        }

        public override void RemoveTab(ToolWindow oldTab)
        {
            TabNode node;
            if (this.nodes.TryGetValue(oldTab, out node))
            {
                // remove the tab
                this.nodes.Remove(oldTab);

                // remove the node from the circular list
                if (this.CurrentTab == oldTab && node.Next == node && node.Prev == node)
                {
                    this.CurrentTab = null;
                }
                else
                {
                    node.Prev.Next = node.Next;
                    node.Next.Prev = node.Prev;
                    this.CurrentTab = node.Prev.Window;
                }

            }
        }

        public override void MoveToNextTab()
        {
        }

        public override void MoveToPrevTab()
        {
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        private IDictionary<ToolWindow, TabNode> nodes = new Dictionary<ToolWindow, TabNode>();

        public class TabNode
        {
            public ToolWindow Window { get; set; }
            public TabNode Next { get; set; }
            public TabNode Prev { get; set; }
        }
    }
}
