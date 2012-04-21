using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FlipCSharp
{
    public class CsAutomate
    {
        

        // Get a handle to an application window.
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Activate an application window.
        [DllImport("USER32.DLL")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        private const int WM_SYSCOMMAND = 0x0112;


        private const int SC_CLOSE = 0xF060;
        private const int SC_MINIMIZE = 0xF020;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_RESTORE = 0xF120;


        public static void WindowClose(IntPtr hWnd)
        {
            SendMessage(hWnd, WM_SYSCOMMAND, SC_CLOSE, 0); 
        }
        public static void WindowMaximize(IntPtr hWnd)
        {
            SendMessage(hWnd, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
        }
        public static IntPtr GetPaint(string sTitle)
        {
            //return FindWindow("MSPaintApp", null);//"scratch.bmp - Paint"); //className of it
            return FindWindow(null, sTitle);//"scratch.bmp - Paint");
        }
        public static bool ActivatePaint(string sTitle)
        {
            //IntPtr p = FindWindow("MSPaintApp", null); //className of it
          //  IntPtr p = FindWindow(null, sTitle);
            IntPtr p = FindWindow(sTitle, null);
            if (p == IntPtr.Zero)
            {
                System.Windows.Forms.MessageBox.Show("Window not found");
                return false;
            }
            SetForegroundWindow(p);
            return true;
        }
        public static void Send(string s)
        {
            SendKeys.SendWait(s);
            //SendKeys.Send(s);
        }



        protected const int WM_CHANGEUISTATE = 0x00000127;
        protected const int UIS_SET = 1;
        protected const int UIS_CLEAR = 2;

        protected const short UISF_HIDEFOCUS = 0x0001;
        protected const short UISF_HIDEACCEL = 0x0002;
        protected const short UISF_ACTIVE = 0x0004;
        public static void MakeFocusInvisible(Control c)
        {
            SendMessage(c.Handle, WM_CHANGEUISTATE, MAKELONG(UIS_SET, UISF_HIDEFOCUS), 0);
        }
        private static int MAKELONG(int wLow, int wHigh)
        {
            int low = (int)LOWORD(wLow);
            short high = LOWORD(wHigh);
            int product = 0x00010000 * (int)high;
            int makeLong = (int)(low | product);
            return makeLong;
        }
        private static short LOWORD(int dw)
        {
            short loWord = 0;
            ushort andResult = (ushort)(dw & 0x00007FFF);
            ushort mask = 0x8000;
            if ((dw & 0x8000) != 0)
                loWord = (short)(mask | andResult);
            else
                loWord = (short)andResult;
            return loWord;
        }
    }
}
