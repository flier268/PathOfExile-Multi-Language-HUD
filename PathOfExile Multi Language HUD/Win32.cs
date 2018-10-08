using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;

namespace PathOfExile_Multi_Language_HUD
{
    public class Win32
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
        struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }
        public static TimeSpan GetLastInput()
        {
            LASTINPUTINFO plii = new LASTINPUTINFO();
            plii.cbSize = (uint)Marshal.SizeOf(plii);

            if (GetLastInputInfo(ref plii))
                //會把最後一次操作的時間寫入在dwTime內  
                return TimeSpan.FromMilliseconds(Environment.TickCount - plii.dwTime);
            //得到的數字都是秒數,轉換成TimeSpan型態比較好用
            else
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}
