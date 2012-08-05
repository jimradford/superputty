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

        public static readonly ITabSwitchStrategy[] Strategies;
        static TabSwitcher()
        {
            List<ITabSwitchStrategy> strats = new List<ITabSwitchStrategy>();
            strats.Add(new VisualOrderTabSwitchStrategy());
            strats.Add(new OpenOrderTabSwitchStrategy());
            strats.Add(new MRUTabSwitchStrategy());
            Strategies = strats.ToArray();
        }

        public static ITabSwitchStrategy StrategyFromTypeName(String typeName)
        {
            ITabSwitchStrategy strategy = Strategies[0];
            try
            {
                Type t = Type.GetType(typeName);
                if (t != null)
                {
                    strategy = (ITabSwitchStrategy) Activator.CreateInstance(t);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error parsing strategy, defaulting to Visual: typeName=" + typeName, ex);
            }
            return strategy;
        }

        public TabSwitcher(DockPanel dockPanel)
        {
            this.Documents = new List<ToolWindow>();

            this.DockPanel = dockPanel;
            this.DockPanel.ContentAdded += DockPanel_ContentAdded;
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
                        Log.InfoFormat("Cleaning up old strategy: {0}", this.tabSwitchStrategy.Description);
                        this.tabSwitchStrategy.Dispose();
                    }

                    // set and init new one
                    this.tabSwitchStrategy = value;
                    if (value != null)
                    {
                        Log.InfoFormat("Initialing new strategy: {0}", this.tabSwitchStrategy.Description);
                        this.tabSwitchStrategy.Initialize(this.DockPanel);
                        foreach (IDockContent doc in this.DockPanel.Documents)
                        {
                            this.AddDocument((ToolWindow)doc);
                        }
                        this.CurrentDocument = this.CurrentDocument ?? this.ActiveDocument;
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
                this.IsSwitchingTabs = false;
            }
        }

        void DockPanel_ContentAdded(object sender, DockContentEventArgs e)
        {
            this.DockPanel.BeginInvoke(new Action(
                delegate
                {
                    if (e.Content.DockHandler.DockState == DockState.Document)
                    {
                        ToolWindow window = (ToolWindow)e.Content;
                        this.AddDocument(window);
                        window.FormClosed += window_FormClosed;
                    }
                }));
        }

        void window_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            ToolWindow window = (ToolWindow)sender;
            this.RemoveDocument((ToolWindow)sender);
        }

        void AddDocument(ToolWindow tab)
        {
            this.TabSwitchStrategy.AddTab(tab);
        }

        void RemoveDocument(ToolWindow tab)
        {
            this.TabSwitchStrategy.RemoveTab(tab);
        }

        public bool MoveToNextDocument()
        {
            this.IsSwitchingTabs = true;
            return this.TabSwitchStrategy.MoveToNextTab();
        }

        public bool MoveToPrevDocument()
        {
            this.IsSwitchingTabs = true;
            return this.TabSwitchStrategy.MoveToPrevTab();
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
            foreach (ToolWindow win in this.Documents)
            {
                win.FormClosed -= this.window_FormClosed;
            }
        }

        public ToolWindow ActiveDocument { get { return (ToolWindow)this.DockPanel.ActiveDocument; } }
        public DockPanel DockPanel { get; private set; }
        public IList<ToolWindow> Documents { get; private set; }
        public bool IsSwitchingTabs { get; set; }

        ITabSwitchStrategy tabSwitchStrategy;
        ToolWindow currentDocument;
    } 
    #endregion

    #region ITabSwitchStrategy
    public interface ITabSwitchStrategy : IDisposable
    {
        string Description { get; }
        void Initialize(DockPanel panel);
        void AddTab(ToolWindow tab);
        void RemoveTab(ToolWindow tab);
        void SetCurrentTab(ToolWindow window);

        bool MoveToNextTab();
        bool MoveToPrevTab();
    }
    #endregion

    #region AbstractOrderedTabSwitchStrategy
    public abstract class AbstractOrderedTabSwitchStrategy : ITabSwitchStrategy
    {

        protected AbstractOrderedTabSwitchStrategy(string desc)
        {
            this.Description = desc;
        }

        public void Initialize(DockPanel panel) 
        {
            this.DockPanel = panel;
        }

        public void AddTab(ToolWindow tab) {}
        public void RemoveTab(ToolWindow tab) {}

        public bool MoveToNextTab()
        {
            bool res = false;
            List<IDockContent> docs = GetDocuments();
            int idx = docs.IndexOf(this.DockPanel.ActiveDocument);
            if (idx != -1)
            {
                ToolWindow winNext = (ToolWindow)docs[idx == docs.Count - 1 ? 0 : idx + 1];
                winNext.Activate();
                res = true;
            }
            return res;
        }

        public bool MoveToPrevTab()
        {
            bool res = false;
            List<IDockContent> docs = GetDocuments();
            int idx = docs.IndexOf(this.DockPanel.ActiveDocument);
            if (idx != -1)
            {
                ToolWindow winPrev = (ToolWindow)docs[idx == 0 ? docs.Count - 1 : idx - 1];
                winPrev.Activate();
                res = true;
            }
            return res;
        }

        protected abstract List<IDockContent> GetDocuments();

        public void SetCurrentTab(ToolWindow window)  { }
        public void Dispose() {}

        protected DockPanel DockPanel { get; set; }
        public string Description { get; protected set; }
    }
    #endregion

    #region VisualOrderTabSwitchStrategy
    public class VisualOrderTabSwitchStrategy : AbstractOrderedTabSwitchStrategy
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(VisualOrderTabSwitchStrategy));

        public VisualOrderTabSwitchStrategy() : 
            base("Visual: Left-to-Right, Top-to-Bottom") { }

        protected override List<IDockContent> GetDocuments()
        {
            List<IDockContent> docs = new List<IDockContent>();
            if (this.DockPanel.Contents.Count > 0 && this.DockPanel.Panes.Count > 0)
            {
                List<DockPane> panes = new List<DockPane>(this.DockPanel.Panes);
                panes.Sort((x, y) =>
                {
                    int res = x.Top.CompareTo(y.Top);
                    return res == 0 ? x.Left.CompareTo(y.Left) : res;
                });
                foreach (DockPane pane in panes)
                {
                    if (pane.Appearance == DockPane.AppearanceStyle.Document)
                    {
                        foreach (IDockContent content in pane.Contents)
                        {
                            if (content.DockHandler.DockState == DockState.Document)
                            {
                                docs.Add(content);
                            }
                        }
                        //Log.InfoFormat("\tPane: contents={0}, L={1}, T={2}", pane.Contents.Count, pane.Left, pane.Top);
                        //foreach (IDockContent content in pane.Contents) { //Log.Info("\t\t" + content.DockHandler.TabText); }
                    }
                }
            }
            return docs;
        }
    }
    #endregion

    #region OpenOrderTabSwitchStrategy
    public class OpenOrderTabSwitchStrategy : AbstractOrderedTabSwitchStrategy
    {
        public OpenOrderTabSwitchStrategy() :
            base("Open: In the order sessions are opened.") { }

        protected override List<IDockContent> GetDocuments()
        {
            return new List<IDockContent>(this.DockPanel.DocumentsToArray());
        }
    }

    #endregion

    #region MRUTabSwitchStrategy
    public class MRUTabSwitchStrategy : ITabSwitchStrategy
    {
        public string Description
        {
            get { return "MRU: Similar to Windows Alt-Tab"; }
        }

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

        public bool MoveToNextTab()
        {
            bool res = false;
            if (this.TransitioningTab == null)
            {
                this.TransitioningTab = this.CurrentTab;
            }

            if (this.TransitioningTab != null)
            {
                this.TransitioningTab = this.TransitioningTab.Next;
                this.TransitioningTab.Window.Activate();
                res = true;
            }
            return res;
        }

        public bool MoveToPrevTab()
        {
            bool res = false;
            if (this.TransitioningTab == null)
            {
                this.TransitioningTab = this.CurrentTab;
            }

            if (this.TransitioningTab != null)
            {
                this.TransitioningTab = this.TransitioningTab.Prev;
                this.TransitioningTab.Window.Activate();
                res = true;
            }
            return res;
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
                this.TransitioningTab = null;
            }
        }

        public void Dispose() { }

        DockPanel DockPanel { get; set; }
        TabNode CurrentTab { get; set; }
        TabNode TransitioningTab { get; set; }

        private IDictionary<ToolWindow, TabNode> nodes = new Dictionary<ToolWindow, TabNode>();

        public class TabNode
        {
            public ToolWindow Window { get; set; }
            public TabNode Next { get; set; }
            public TabNode Prev { get; set; }
        }
    }
    #endregion
}
