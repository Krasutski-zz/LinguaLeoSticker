using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LinguaLeoSticker
{
    class GlobalKeyboardHook
    {
        public delegate bool KeyHookDelegate(int keyStatus, Keys activeKey);

        public event KeyHookDelegate KeyHookEvt;

        #region Definition of Structures, Constants and Delegates

        public delegate int KeyboardHookProc(int nCode, int wParam, ref GlobalKeyboardHookStruct lParam);

        public struct GlobalKeyboardHookStruct
        {
            public int VkCode;
            public int ScanCode;
            public int Flags;
            public int Time;
            public int DwExtraInfo;
        }

        private const int WhKeyboardLl = 13;

        public enum KeyboardMessage
        {
            WmKeydown = 0x0100,
            WmKeyup = 0x0101,
            WmChar = 0x0102,
            WmDeadchar = 0x0103,
            WmSyskeydown = 0x0104,
            WmSyskeyup = 0x0105,
            WmSyschar = 0x0106,
            WmSysdeadchar = 0x0107,
            WmKeyfirst = WmKeydown,
            WmKeylast = 0x0108,
        }

        #endregion

        #region Events

        //public event KeyEventHandler KeyDown;
        //public event KeyEventHandler KeyUp;

        #endregion

        #region Instance Variables

        public List<Keys> HookedKeys = new List<Keys>();
        private IntPtr _hookHandle = IntPtr.Zero;
        #endregion

        #region DLL Imports

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int hookId, KeyboardHookProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern bool UnhookWindowsHookEx(IntPtr hookHandle);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern int CallNextHookEx(IntPtr hookHandle, int nCode, int wParam, ref GlobalKeyboardHookStruct lParam);




        #endregion

        #region Public Methods

        public int HookProc(int nCode, int wParam, ref GlobalKeyboardHookStruct lParam)
        {
            Keys keyPresed = (Keys)lParam.VkCode;

            if (KeyHookEvt != null)
            {
                if (KeyHookEvt(wParam, keyPresed))
                {
                    return 1;
                }
            }

            return CallNextHookEx(_hookHandle, nCode, wParam, ref lParam);
        }

        KeyboardHookProc _hookProc;
        public void Hook()
        {
            _hookProc = HookProc;
            LoadLibrary("user32");

            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                _hookHandle = SetWindowsHookEx(WhKeyboardLl, _hookProc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }


        public void Unhook()
        {
            UnhookWindowsHookEx(_hookHandle);
        }

        #endregion

        #region Constructors and Destructors

        public GlobalKeyboardHook()
        {
            Hook();
        }

        ~GlobalKeyboardHook()
        {
            Unhook();
        }

        #endregion
    }
}
