/*
 * Copyright (c) 2009 - 2015 Jim Radford http://www.jimradford.com
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions: 
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

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
            List<ITabSwitchStrategy> strats = new List<ITabSwitchStrategy>
            {
                new VisualOrderTabSwitchStrategy(),
                new OpenOrderTabSwitchStrategy(),
                new MRUTabSwitchStrategy()
            };
            //strats.Add(new MRUTabSwitchStrategyOld());
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
                    strategy = (ITabSwitchStrategy)Activator.CreateInstance(t);
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
                //Log.Info("Setting current doc: " + value);
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
            tab.FormClosed += window_FormClosed;
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

        public void Dispose()
        {
            this.DockPanel.ContentAdded -= DockPanel_ContentAdded;
            foreach (IDockContent content in this.DockPanel.Documents)
            {
                ToolWindow win = content as ToolWindow;
                if (win != null)
                {
                    win.FormClosed -= this.window_FormClosed;
                }
            }
        }

        public IList<IDockContent> Documents => this.tabSwitchStrategy.GetDocuments();

        public ToolWindow ActiveDocument => (ToolWindow)this.DockPanel.ActiveDocument;
        public DockPanel DockPanel { get; private set; }
        public bool IsSwitchingTabs { get; set; }

        ITabSwitchStrategy tabSwitchStrategy;
        ToolWindow currentDocument;
    }
    #endregion

    #region ITabSwitchStrategy
    public interface ITabSwitchStrategy : IDisposable
    {
        string Description { get; }
        IList<IDockContent> GetDocuments();
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

        public void AddTab(ToolWindow tab) { }
        public void RemoveTab(ToolWindow tab) { }

        public bool MoveToNextTab()
        {
            bool res = false;
            IList<IDockContent> docs = GetDocuments();
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
            IList<IDockContent> docs = GetDocuments();
            int idx = docs.IndexOf(this.DockPanel.ActiveDocument);
            if (idx != -1)
            {
                ToolWindow winPrev = (ToolWindow)docs[idx == 0 ? docs.Count - 1 : idx - 1];
                winPrev.Activate();
                res = true;
            }
            return res;
        }

        public abstract IList<IDockContent> GetDocuments();

        public void SetCurrentTab(ToolWindow window) { }
        public void Dispose() { }

        protected DockPanel DockPanel { get; set; }
        public string Description { get; protected set; }
    }
    #endregion

    #region VisualOrderTabSwitchStrategy
    public class VisualOrderTabSwitchStrategy : AbstractOrderedTabSwitchStrategy
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(VisualOrderTabSwitchStrategy));

        public VisualOrderTabSwitchStrategy() :
            base("Visual: Left-to-Right, Top-to-Bottom")
        { }

        public override IList<IDockContent> GetDocuments()
        {
            return GetDocuments(this.DockPanel);
        }

        /// <summary>Get a List containing session panels from a <seealso cref="DockPanel"/></summary>
        /// <param name="dockPanel">The DockPanel parent containing the children panels</param>
        /// <returns>A <seealso cref="IList{T}"/> containing open session panels of type <seealso cref="ctlPuttyPanel"/></returns>
        public static IList<IDockContent> GetDocuments(DockPanel dockPanel)
        {
            List<IDockContent> docs = new List<IDockContent>();
            if (dockPanel.Contents.Count > 0 && dockPanel.Panes.Count > 0)
            {
                List<DockPane> panes = new List<DockPane>(dockPanel.Panes);
                panes.Sort((x, y) =>
                {
                    int res = x.Top.CompareTo(y.Top);
                    return res == 0 ? x.Left.CompareTo(y.Left) : res;
                });
                foreach (DockPane pane in panes)
                {
                    docs.AddRange(pane.Contents.OfType<ctlPuttyPanel>().Cast<IDockContent>());
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
            base("Open: In the order sessions are opened.")
        { }

        public override IList<IDockContent> GetDocuments()
        {
            return new List<IDockContent>(this.DockPanel.DocumentsToArray());
        }
    }

    #endregion

    #region MRUTabSwitchStrategy
    public class MRUTabSwitchStrategy : ITabSwitchStrategy
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MRUTabSwitchStrategy));

        public string Description => "MRU: Similar to Windows Alt-Tab";

        public void Initialize(DockPanel panel)
        {
            this.DockPanel = panel;
        }

        public void AddTab(ToolWindow newTab)
        {
            Log.InfoFormat("AddTab: {0}", newTab.Text);
            this.docs.Add(newTab);
        }

        public void RemoveTab(ToolWindow oldTab)
        {
            this.docs.Remove(oldTab);
        }

        public bool MoveToNextTab()
        {
            bool res = false;
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
            int idx = docs.IndexOf(this.DockPanel.ActiveDocument);
            if (idx != -1)
            {
                ToolWindow winNext = (ToolWindow)docs[idx == docs.Count - 1 ? 0 : idx + 1];
                winNext.Activate();
                res = true;
            }
            return res;
        }

        public void SetCurrentTab(ToolWindow window)
        {
            if (window != null)
            {
                if (this.docs.Contains(window))
                {
                    this.docs.Remove(window);
                    this.docs.Insert(0, window);
                    if (Log.IsDebugEnabled)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (IDockContent doc in docs)
                        {
                            sb.Append(((ToolWindow)doc).Text).Append(", ");
                        }
                        Log.DebugFormat("Tabs: {0}", sb.ToString().TrimEnd(','));
                    }
                }
            }
        }

        public IList<IDockContent> GetDocuments()
        {
            return this.docs;
        }

        public void Dispose() { }

        DockPanel DockPanel { get; set; }

        private IList<IDockContent> docs = new List<IDockContent>();

    }
    #endregion

}
