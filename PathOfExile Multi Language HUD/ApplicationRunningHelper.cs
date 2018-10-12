/*Clone from Poe整理倉庫v2*/
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Poe整理倉庫v2
{

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
        public POINT(POINT p)
        {
            this.X = p.X;
            this.Y = p.Y;
        }
        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public POINT(System.Drawing.Point pt) : this(pt.X, pt.Y) { }

        public static implicit operator System.Drawing.Point(POINT p)
        {
            return new System.Drawing.Point(p.X, p.Y);
        }

        public static implicit operator POINT(System.Drawing.Point p)
        {
            return new POINT(p.X, p.Y);
        }
        public override bool Equals(object obj)
        {
            if (((POINT)obj).X == X && ((POINT)obj).Y == Y)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


    public class ApplicationHelper
    {
        public static Process currentProcess;

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        public static extern int GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        public static bool IsPathOfExileTop(IntPtr CallerHandle)
        {
            var arrProcesses = Process.GetProcessesByName("PathOfExile");
            if (arrProcesses.Length <= 0)
                arrProcesses = Process.GetProcessesByName("PathOfExile_x64");
            else if (arrProcesses.Length <= 0)
                arrProcesses = Process.GetProcessesByName("PathOfExileSteam");
            else if (arrProcesses.Length <= 0)
                arrProcesses = Process.GetProcessesByName("PathOfExile_x64Steam");
            if (arrProcesses.Length > 0)
            {
                currentProcess = arrProcesses[0];
                IntPtr hWnd = arrProcesses[0].MainWindowHandle;
                int foregroundwindowHandle = GetForegroundWindow();
                if (foregroundwindowHandle == (int)hWnd || foregroundwindowHandle == (int)CallerHandle)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public static IntPtr OpenPathOfExile()
        {
            const int swRestore = 9;
            var arrProcesses = Process.GetProcessesByName("PathOfExile");
            if (arrProcesses.Length <= 0)
                arrProcesses = Process.GetProcessesByName("PathOfExile_x64");
            if (arrProcesses.Length <= 0)
                arrProcesses = Process.GetProcessesByName("PathOfExileSteam");
            if (arrProcesses.Length <= 0)
                arrProcesses = Process.GetProcessesByName("PathOfExile_x64Steam");
            if (arrProcesses.Length > 0)
            {
                currentProcess = arrProcesses[0];
                IntPtr hWnd = arrProcesses[0].MainWindowHandle;
                if (IsIconic(hWnd))
                    ShowWindowAsync(hWnd, swRestore);
                SetForegroundWindow(hWnd);
                return hWnd;
            }
            return IntPtr.Zero;
        }
        WinEventDelegate dele2;
        public ApplicationHelper()
        {
            dele = new WinEventDelegate(WinEventProc);
            dele2 = new WinEventDelegate(WinEventProc_Resize);
            IntPtr m_hhook_MINIMIZEEND = SetWinEventHook(EVENT_SYSTEM_MINIMIZEEND, EVENT_SYSTEM_MINIMIZEEND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);
            IntPtr m_hhook_FOREGROUND = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);
            IntPtr m_hhook_MOVESIZEEND = SetWinEventHook(EVENT_SYSTEM_MOVESIZEEND, EVENT_SYSTEM_MOVESIZEEND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);
            //Event Constants沒有最大化或回復為原本大小的事件，因此只能在EVENT_OBJECT_LOCATIONCHANGE的子事件找
            IntPtr m_hhook_LOCATIONCHANGE = SetWinEventHook(EVENT_OBJECT_LOCATIONCHANGE, EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, dele2, 0, 0, WINEVENT_OUTOFCONTEXT);
        }
        public delegate void MyEventHandler();
        public MyEventHandler ForegroundWindowChanged;

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            ForegroundWindowChanged?.Invoke();
        }
        private void WinEventProc_Resize(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (EVENT_OBJECT_LOCATIONCHANGE == eventType)
            {
                WINDOWPLACEMENT wp=new WINDOWPLACEMENT();
                GetWindowPlacement((int)hwnd, ref wp);
                if (wp.showCmd==SW_SHOWMAXIMIZED || wp.showCmd== SW_SHOWNORMAL)
                {
                    ForegroundWindowChanged?.Invoke();
                }
            }
        }
        static WinEventDelegate dele = null;
        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        private const uint EVENT_SYSTEM_MINIMIZEEND = 0x17;
        private const uint EVENT_SYSTEM_MOVESIZEEND = 0x00B;
        private const uint SW_SHOWNORMAL = 1;
        private const uint SW_SHOWMAXIMIZED = 3;
        

        private const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;

        private struct WINDOWPLACEMENT
        {
            internal uint length;
            internal uint flags;
            internal uint showCmd;
            internal POINT ptMinPosition;
            internal POINT ptMaxPosition;
            internal RECT rcNormalPosition;
        }
        [DllImport("user32.dll", EntryPoint = "GetWindowPlacement")]
        static extern int GetWindowPlacement(
           int hwnd,
           ref WINDOWPLACEMENT lpwndpl
        );


        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT Rect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetClientRect(IntPtr hWnd, ref RECT Rect);


        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);



        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int ScreenToClient(IntPtr hWnd, out POINT pt);


        public static RECT PathOfExileDimentions
        {
            get
            {
                RECT clientRect = new RECT();
                GetClientRect(currentProcess.MainWindowHandle, ref clientRect);

                POINT point;
                ScreenToClient(currentProcess.MainWindowHandle, out point);

                RECT rect = new RECT();
                rect.Left = point.X * -1 + clientRect.Left;
                rect.Right = point.X * -1 + clientRect.Right;
                rect.Top = point.Y * -1 + clientRect.Top;
                rect.Bottom = point.Y * -1 + clientRect.Bottom;

                return rect;
            }
        }
        public static RECT PathOfExilePosition(POINT p)
        {
            RECT rect = PathOfExileDimentions;
            rect.Left = p.X - rect.Left;
            rect.Right = rect.Right- p.X;
            rect.Top = p.Y - rect.Top;
            rect.Bottom = rect.Bottom - p.Y;

            return rect;

        }
    }
}