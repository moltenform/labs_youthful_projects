//SetKeyState
//Ben Fisher, 2008, GPL

// Parts of the following code are adapted from
// http://www.codeproject.com/KB/statusbar/update_toggle_key_status.aspx
// by Bilal Haider, GPL

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace Blinkbeat
{
    class SetKeyState
    {
        /// <summary>
        /// This function retrieves the status of the specified virtual key.
        /// The status specifies whether the key is up, down.
        /// </summary>
        /// <param name="keyCode">Specifies a key code for the button to me checked</param>
        /// <returns>Return value will be 0 if off and 1 if on</returns>
        [DllImport("user32.dll")]
        private static extern short GetKeyState(int keyCode);

        /// <summary>
        /// This function is useful to simulate Key presses to the window with focus.
        /// </summary>
        /// <param name="bVk">Specifies a virtual-key code. The code must be a value in the range 1 to 254.</param>
        /// <param name="bScan">Specifies a hardware scan code for the key.</param>
        /// <param name="dwFlags"> Specifies various aspects of function operation. This parameter can be one or more of the following values.
        ///                         <code>KEYEVENTF_EXTENDEDKEY</code> or <code>KEYEVENTF_KEYUP</code>
        ///                         If specified, the key is being released. If not specified, the key is being depressed.</param>
        /// <param name="dwExtraInfo">Specifies an additional value associated with the key stroke</param>
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);


        public static bool GetCapsState()
        {
            return GetToggleState((int)Keys.CapsLock);
        }
        public static bool GetNumState()
        {
            return GetToggleState((int)Keys.NumLock);
        }
        public static bool GetScrollState()
        {
            return GetToggleState((int)145); //scroll lock
        }
        private static bool GetToggleState(int keyId)
        {
            int state = (GetKeyState(keyId));
            return state != 0;
        }

        public static void SetCapsState(bool value)
        {
            SetToggleState((int)Keys.CapsLock, value);
        }
        public static void SetNumState(bool value)
        {
            SetToggleState((int)Keys.NumLock, value);
        }
        public static void SetScrollState(bool value)
        {
            SetToggleState((int)145, value);
        }
        private static void SetToggleState(int keyId, bool value)
        {
            bool currentValue = GetToggleState(keyId);
            if (value != currentValue)
            {
                const int KEYEVENTF_EXTENDEDKEY = 0x1;
                const int KEYEVENTF_KEYUP = 0x2;
                keybd_event((byte)keyId, 0x45, KEYEVENTF_EXTENDEDKEY, 0);
                keybd_event((byte)keyId, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            }
        }

    }
}
