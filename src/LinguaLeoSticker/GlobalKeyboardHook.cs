using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace KeyboardHook
{
    class GlobalKeyboardHook
    {
        public delegate bool key_hook_Delegate(int KeyStatus, Keys ActiveKey);

        public event key_hook_Delegate key_hook_evt = null;

        #region Definition of Structures, Constants and Delegates

        public delegate int KeyboardHookProc(int nCode, int wParam, ref GlobalKeyboardHookStruct lParam);

        public struct GlobalKeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        private const int WH_KEYBOARD_LL = 13;

        public enum KeyboardMessage
        {
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_CHAR = 0x0102,
            WM_DEADCHAR = 0x0103,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105,
            WM_SYSCHAR = 0x0106,
            WM_SYSDEADCHAR = 0x0107,
            WM_KEYFIRST = WM_KEYDOWN,
            WM_KEYLAST = 0x0108,
        }

        #endregion

        #region Events

        //public event KeyEventHandler KeyDown;
        //public event KeyEventHandler KeyUp;

        #endregion

        #region Instance Variables

        public List<Keys> HookedKeys = new List<Keys>();
        IntPtr hookHandle = IntPtr.Zero;
        #endregion

        #region DLL Imports

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int hookID, KeyboardHookProc callback, IntPtr hInstance, uint threadID);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern bool UnhookWindowsHookEx(IntPtr hookHandle);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern int CallNextHookEx(IntPtr hookHandle, int nCode, int wParam, ref GlobalKeyboardHookStruct lParam);




        #endregion

        #region Public Methods

        public int hookProc(int nCode, int wParam, ref GlobalKeyboardHookStruct lParam)
        {
            KeyboardMessage Msg = (KeyboardMessage)wParam;

            Keys key_presed = (Keys)lParam.vkCode;

            if (key_hook_evt != null)
            {
                if (key_hook_evt(wParam, key_presed))
                {
                    return 1;
                }
            }

            return CallNextHookEx(hookHandle, nCode, wParam, ref lParam);
        }

        KeyboardHookProc _hookProc;
        public void hook()
        {
            _hookProc = new KeyboardHookProc(hookProc);
            IntPtr hInstance = LoadLibrary("user32");

            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                hookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, _hookProc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }


        public void unhook()
        {
            UnhookWindowsHookEx(hookHandle);
        }

        #endregion

        #region Constructors and Destructors

        public GlobalKeyboardHook()
        {
            hook();
        }

        ~GlobalKeyboardHook()
        {
            unhook();
        }

        #endregion
    }
}
