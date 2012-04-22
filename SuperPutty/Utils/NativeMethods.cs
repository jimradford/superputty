using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace SuperPutty.Utils
{
    public class NativeMethods
    {
        public const short WM_COPYDATA = 74;

        public const int WH_KEYBOARD_LL = 13;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_CHAR = 0x102;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;

        public struct COPYDATA
        {
            public int dwData;
            public uint cbData;
            public IntPtr lpData;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);

        [DllImport("USER32.DLL", EntryPoint = "PostMessageW", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = true,
             CallingConvention = CallingConvention.StdCall)]
        public static extern bool PostMessage(int hwnd, int Msg, int wParam, int lParam);

        [DllImport("USER32.DLL", EntryPoint = "SendMessageW", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = true,
             CallingConvention = CallingConvention.StdCall)]
        public static extern bool SendMessage(int hwnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern short VkKeyScan(char ch);
    }
}
