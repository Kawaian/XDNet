using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace XDNet
{
    public enum TaskbarPosition
    {
        Unknown = -1,
        Left,
        Top,
        Right,
        Bottom,
    }

    public static class TaskbarInfo
    {
        private enum ABS
        {
            AutoHide = 0x01,
            AlwaysOnTop = 0x02,
        }

        private enum AppBarEdge : uint
        {
            Left = 0,
            Top = 1,
            Right = 2,
            Bottom = 3
        }

        private enum AppBarMessage : uint
        {
            New = 0x00000000,
            Remove = 0x00000001,
            QueryPos = 0x00000002,
            SetPos = 0x00000003,
            GetState = 0x00000004,
            GetTaskbarPos = 0x00000005,
            Activate = 0x00000006,
            GetAutoHideBar = 0x00000007,
            SetAutoHideBar = 0x00000008,
            WindowPosChanged = 0x00000009,
            SetState = 0x0000000A,
        }

        private const string ClassName = "Shell_TrayWnd";
        private static APPBARDATA _appBarData;

        static TaskbarInfo()
        {
            _appBarData = new APPBARDATA
            {
                cbSize = (uint)Marshal.SizeOf(typeof(APPBARDATA)),
                hWnd = FindWindow(TaskbarInfo.ClassName, null)
            };
        }

        public static bool AlwaysOnTop
        {
            get
            {
                int state = SHAppBarMessage(AppBarMessage.GetState, ref _appBarData).ToInt32();
                return ((ABS)state).HasFlag(ABS.AlwaysOnTop);
            }
        }

        public static bool AutoHide
        {
            get
            {
                int state = SHAppBarMessage(AppBarMessage.GetState, ref _appBarData).ToInt32();
                return ((ABS)state).HasFlag(ABS.AutoHide);
            }
        }

        public static Rectangle CurrentBounds
        {
            get
            {
                var rect = new RECT();
                if (GetWindowRect(Handle, ref rect))
                    return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);

                return Rectangle.Empty;
            }
        }

        public static Rectangle DisplayBounds
        {
            get
            {
                if (RefreshBoundsAndPosition())
                    return Rectangle.FromLTRB(_appBarData.rect.Left,
                                              _appBarData.rect.Top,
                                              _appBarData.rect.Right,
                                              _appBarData.rect.Bottom);

                return CurrentBounds;
            }
        }

        public static IntPtr Handle
        {
            get { return _appBarData.hWnd; }
        }
        
        public static TaskbarPosition Position
        {
            get
            {
                if (RefreshBoundsAndPosition())
                    return (TaskbarPosition)_appBarData.uEdge;

                return TaskbarPosition.Unknown;
            }
        }

        /// <summary>Hides the taskbar.</summary>
        public static void Hide()
        {
            const int SW_HIDE = 0;
            ShowWindow(Handle, SW_HIDE);
        }
        
        public static void Show()
        {
            const int SW_SHOW = 1;
            ShowWindow(Handle, SW_SHOW);
        }

        private static bool RefreshBoundsAndPosition()
        {
            return SHAppBarMessage(AppBarMessage.GetTaskbarPos, ref _appBarData) != IntPtr.Zero;
        }

        #region DllImports

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern IntPtr SHAppBarMessage(AppBarMessage dwMessage, [In] ref APPBARDATA pData);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hwnd, int command);

        #endregion DllImports

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public AppBarEdge uEdge;
            public RECT rect;
            public int lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
