using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyboardHook
{
    class GlobalKeyboardHook
    {
        public delegate void key_hook_Delegate(Keys key);

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

        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;
        const int WH_KEYBOARD_LL = 13;

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

            if (key_hook_evt != null)
            {
                if (wParam == WM_KEYUP)
                {
                    Keys key = (Keys)lParam.vkCode;
                    key_hook_evt(key);
                }
            }

            return CallNextHookEx(hookHandle, nCode, wParam, ref lParam);
        }

        KeyboardHookProc _hookProc;
        public void hook()
        {
            _hookProc = new KeyboardHookProc(hookProc);
            IntPtr hInstance = LoadLibrary("user32");
            hookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, _hookProc, hInstance, 0);
            //IntPtr hInstance = LoadLibrary("user32");
            //hookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, hInstance, 0);
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
