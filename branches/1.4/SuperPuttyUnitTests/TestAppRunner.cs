using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using log4net;
using System.IO;
using System.Threading;
using NUnit.Gui;

namespace SuperPuttyUnitTests
{
    public partial class TestAppRunner : Form
    {
        public const string NoAutoStart = "<Disable>";
        private static readonly string AutoStartFile = Path.Combine(Path.GetTempPath(), "SuperPutty-UnitTest-AutoStart.txt");

        private static readonly ILog Log = LogManager.GetLogger(typeof(TestAppRunner));

        public TestAppRunner()
        {
            InitializeComponent();

            AddViewButtons(null);
        }

        void AddViewButtons(MethodInfo methodAutoStart)
        {
            // special case for no auto-start
            this.comboAutoStart.Items.Add(NoAutoStart);
            this.comboAutoStart.SelectedItem = NoAutoStart;

            MethodInfo[] methods = TestViewAttribute.GetAllTestViews(Assembly.GetEntryAssembly());
            Array.Reverse(methods);
            Log.InfoFormat("Found {0} test views", methods.Length);
            foreach (MethodInfo mi in methods)
            {
                Log.InfoFormat("Adding TestView: type={0}, method={1}", mi.DeclaringType, mi.Name);

                string item = ToComboItem(mi);
                this.comboAutoStart.Items.Add(item);
                if (mi == methodAutoStart)
                {
                    this.comboAutoStart.SelectedItem = item;
                }
                Button btn = new Button
                {
                    Text = string.Format("{0}.{1}", mi.DeclaringType.FullName, mi.Name),
                    Tag = mi,
                    Dock = DockStyle.Top, 
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft
                };
                btn.Click += (s, e) => RunTestView((MethodInfo)((Button)s).Tag);
                groupBoxViews.Controls.Add(btn);
            }
        }

        public static string ToComboItem(MethodInfo mi)
        {
            return string.Format("{0}-{1}", mi.DeclaringType, mi.Name);
        }

        void RunTestView(MethodInfo method)
        {
            try
            {
                Object instance = Activator.CreateInstance(method.DeclaringType);
                method.Invoke(instance, null);

                // set as selected item
                this.comboAutoStart.SelectedItem = ToComboItem(method);

                // kill forms together
                if (Form.ActiveForm != this)
                {
                    Form.ActiveForm.FormClosed += (s, e) => this.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Could not start test view: " + method, ex);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // auto start if method found
            if (File.Exists(AutoStartFile))
            {
                string autoStartType = File.ReadAllText(AutoStartFile);
                try
                {
                    string[] parts = autoStartType.Split('-');
                    if (parts.Length == 2)
                    {
                        Type t = Type.GetType(parts[0]);
                        if (t != null)
                        {
                            this.comboAutoStart.SelectedItem = autoStartType;
                            this.BeginInvoke(
                                new Action<MethodInfo>((mi) =>
                                {
                                    RunTestView(mi);
                                }), 
                                t.GetMethod(parts[1]));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Could not autostart: " + AutoStartFile, ex);
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            // save autostart view
            string item = (string) this.comboAutoStart.SelectedItem;
            if (item != NoAutoStart)
            {
                File.WriteAllText(AutoStartFile, item);
            }
            else
            {
                File.Delete(AutoStartFile);
            }
        }
    }

    public class NUnitRunner
    {
        [TestView]
        public void RunGui()
        {
            Thread t = new Thread((x) =>
            {
                AppEntry.Main(new[] { Assembly.GetEntryAssembly().Location });
            });

            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class TestViewAttribute : Attribute
    {
        public static MethodInfo[] GetAllTestViews(Assembly assembly)
        {
            List<MethodInfo> methods = new List<MethodInfo>();

            foreach (Type type in assembly.GetTypes())
            {
                foreach (MethodInfo mi in type.GetMethods())
                {
                    foreach (Attribute attrib in mi.GetCustomAttributes(true))
                    {
                        if (attrib is TestViewAttribute)
                        {
                            methods.Add(mi);
                        }
                    }
                }
            }
            methods.Sort((a, b) =>
            {
                if (a == b) return 0;
                if (a == null) return 1;
                if (b == null) return -1;
                int i = Comparer<string>.Default.Compare(a.DeclaringType.FullName, b.DeclaringType.FullName);
                return i == 0 ? Comparer<string>.Default.Compare(a.Name, b.Name) : i;
            });
            return methods.ToArray();
        }
    }

}
