using System;
using System.Windows;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;

using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using System.Windows.Resources;
using System.IO;
using ToastNotifications.Core;
using System.Windows.Forms;

namespace XDNet
{
    public class PrimaryTaskBarPositionProvider : IPositionProvider
    {
        public event EventHandler UpdatePositionRequested;
        public event EventHandler UpdateEjectDirectionRequested;
        public event EventHandler UpdateHeightRequested;

        public Window ParentWindow => null;

        public EjectDirection EjectDirection { private set; get; }
        
        public System.Drawing.Rectangle Screen => System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }

        Corner _corner;
        Corner Corner
        {
            get => _corner;
            set
            {
                _corner = value;
                switch (_corner)
                {
                    case Corner.TopRight:
                    case Corner.TopLeft:
                        EjectDirection = EjectDirection.ToBottom;
                        break;
                    case Corner.BottomRight:
                    case Corner.BottomLeft:
                    case Corner.BottomCenter:
                        EjectDirection = EjectDirection.ToTop;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_corner), _corner, null);
                }
            }
        }

        public PrimaryTaskBarPositionProvider(double offsetX, double offsetY)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public void Dispose()
        {

        }

        public double GetHeight()
        {
            return Screen.Height;
        }

        public Point GetPosition(double actualPopupWidth, double actualPopupHeight)
        {
            switch (TaskbarInfo.Position)
            {
                case TaskbarPosition.Left:
                    Corner = Corner.BottomLeft;
                    break;
                case TaskbarPosition.Top:
                    Corner = Corner.TopRight;
                    break;
                case TaskbarPosition.Right:
                    Corner = Corner.BottomRight;
                    break;
                case TaskbarPosition.Bottom:
                    Corner = Corner.BottomRight;
                    break;
                case TaskbarPosition.Unknown:
                default:
                    break;
            }

            switch (Corner)
            {
                case Corner.TopRight:
                    return new Point(Screen.Width - OffsetX - actualPopupWidth, OffsetY);
                case Corner.TopLeft:
                    return new Point(OffsetX, OffsetY);
                case Corner.BottomRight:
                    return new Point(Screen.Width - OffsetX - actualPopupWidth, Screen.Height - OffsetY - actualPopupHeight);
                case Corner.BottomLeft:
                    return new Point(OffsetX, Screen.Height - OffsetY - actualPopupHeight);
                case Corner.BottomCenter:
                    return new Point((Screen.Width - OffsetX - actualPopupWidth) / 2, Screen.Height - OffsetY - actualPopupHeight);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public partial class MainWindow : Window
    {
        Notifier notifier = null;
        HotKey globalHotKey = null;
        EmojiLoader emojiLoader = null;
        IPositionProvider p = new PrimaryTaskBarPositionProvider(15, 15);

        public MainWindow()
        {
            InitializeComponent();
            SetupTrayIcon();
            SetupNotifications();
            
            emojiLoader = new EmojiLoader();
            emojiLoader.FileNotFound += (s, arg) =>
            {
                notifier.ShowWarning(arg);
            };
            emojiLoader.Load("emojis.txt");
            globalHotKey = new HotKey(Key.D, KeyModifier.Shift | KeyModifier.Ctrl, OnHotKeyHandler);

            Hide();
        }

        void Shutdown()
        {
            Environment.Exit(0);
        }

        void SetupTrayIcon()
        {
            var icon = new NotifyIcon();

            StreamResourceInfo sri = System.Windows.Application.GetResourceStream(new Uri("/XDNet;component/xdnet.ico", UriKind.Relative));
            if (sri != null)
            {
                using (Stream s = sri.Stream)
                    icon.Icon = new System.Drawing.Icon(s);
            }
            else
            {
                icon.Icon = System.Drawing.SystemIcons.Error;
            }

            var menu = new ContextMenu();
            
            var exit = new MenuItem("Exit");
            exit.Click += (o, s) => { Shutdown(); };
            menu.MenuItems.Add(exit);

            icon.Text = "XDNet: Emoji generator";
            icon.ContextMenu = menu;
            icon.Visible = true;
            icon.ShowBalloonTip(1, "XDNet is running", "XDNet", ToolTipIcon.None);
        }

        void SetupNotifications()
        {
            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = p;

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(TimeSpan.FromSeconds(0.75), MaximumNotificationCount.FromCount(10));

                cfg.Dispatcher = Dispatcher;
            });
        }

        void OnHotKeyHandler(HotKey hotKey)
        {
            string emoji = emojiLoader.GetRandomEmoji();
            Dispatcher.Invoke(new Action(() => notifier.ShowInformation(emoji + " Copied to clipboard!")));

            System.Windows.Clipboard.SetText(emoji);
        }
    }
}
