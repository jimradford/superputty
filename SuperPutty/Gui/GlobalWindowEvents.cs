using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperPutty.Utils
{
    public class GlobalWindowEventArgs : EventArgs
    {
        public IntPtr hwnd;
    }

    public class GlobalWindowEventProxy
    {
        NativeMethods.WinEventDelegate m_winEventDelegate;
        IntPtr m_hWinEventHook;
        public event EventHandler<GlobalWindowEventArgs> On;
        private uint type;

        public GlobalWindowEventProxy(uint type)
        {
            this.type = type;
            this.m_winEventDelegate = new NativeMethods.WinEventDelegate(WinEventProc);
            this.m_hWinEventHook = NativeMethods.SetWinEventHook(type, type, IntPtr.Zero, this.m_winEventDelegate, 0, 0, NativeMethods.WINEVENT_OUTOFCONTEXT);
        }

        ~GlobalWindowEventProxy()
        {
            NativeMethods.UnhookWinEvent(m_hWinEventHook);
        }

        void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            GlobalWindowEventArgs args = new GlobalWindowEventArgs();
            args.hwnd = hwnd;

            if(this.On != null)
                this.On(this, args);
        }
    }

    public class GlobalWindowEvents
    {
        public GlobalWindowEventProxy ObjectNameChange;
        public GlobalWindowEventProxy SystemSwitchStart;
        public GlobalWindowEventProxy SystemSwitchEnd;
        public GlobalWindowEventProxy SystemForeground;

        public GlobalWindowEvents()
        {
            ObjectNameChange = new GlobalWindowEventProxy((uint)NativeMethods.WinEvents.EVENT_OBJECT_NAMECHANGE);
            SystemSwitchStart = new GlobalWindowEventProxy((uint)NativeMethods.WinEvents.EVENT_SYSTEM_SWITCHSTART);
            SystemSwitchEnd = new GlobalWindowEventProxy((uint)NativeMethods.WinEvents.EVENT_SYSTEM_SWITCHEND);
            SystemForeground = new GlobalWindowEventProxy((uint)NativeMethods.WinEvents.EVENT_SYSTEM_FOREGROUND);
         }
    }
}
