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

namespace XDNet
{
    public partial class MainWindow : Window
    {
        Notifier notifier = null;
        HotKey globalHotKey = null;
        EmojiLoader emojiLoader = null;

        Thread layoutUpdater;

        public MainWindow()
        {
            InitializeComponent();
            SetupTrayIcon();
            SetupNotifications();

            // Load the emojis :3
            emojiLoader = new EmojiLoader("emojis.txt");

            // Create global keybinding
            globalHotKey = new HotKey(Key.D, KeyModifier.Shift | KeyModifier.Ctrl, OnHotKeyHandler);

            // Update layout based on taskbar's position
            layoutUpdater = new Thread(ResetLayoutTh);
            layoutUpdater.Start();
        }

        void Shutdown()
        {
            // resetLayoutRunning = false;

            try
            {
                // TODO(matyas): Thread.join not working for some reason 
                layoutUpdater.Abort();
            }
            catch(Exception)
            {
            }

            Close();
        }

        void SetupTrayIcon()
        {
            var icon = new System.Windows.Forms.NotifyIcon();

            StreamResourceInfo sri = System.Windows.Application.GetResourceStream(new Uri("/XDNet;component/xdnet.ico", UriKind.Relative));
            if (sri != null)
                using (Stream s = sri.Stream)
                    icon.Icon = new System.Drawing.Icon(s);
            else
                icon.Icon = System.Drawing.SystemIcons.Error;

            var menu = new System.Windows.Forms.ContextMenu();

            
            var exit = new System.Windows.Forms.MenuItem("Exit");
            exit.Click += (o, s) => { Shutdown(); };
            menu.MenuItems.Add(exit);

            icon.Text = "XDNet: Emoji generator";
            icon.ContextMenu = menu;
            icon.Visible = true;
            icon.ShowBalloonTip(1, "XDNet is running", "XDNet", System.Windows.Forms.ToolTipIcon.None);
        }

        void SetupNotifications()
        {
            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.TopRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(2),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(3));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        // volatile bool resetLayoutRunning = true;

        void ResetLayoutTh()
        {
            LayoutManager layoutManager = new LayoutManager();

            while (true /*resetLayoutRunning*/)
            {
                Thread.Sleep(1500);
                layoutManager.Update();

                Dispatcher.Invoke(new Action(() => Width = layoutManager.Width));
                Dispatcher.Invoke(new Action(() => Height = layoutManager.Height));
                Dispatcher.Invoke(new Action(() => Left = layoutManager.Left));
                Dispatcher.Invoke(new Action(() => Top = layoutManager.Top));
            }
        }

        void OnHotKeyHandler(HotKey hotKey)
        {
            string emoji = emojiLoader.GetRandomEmoji();
            Dispatcher.Invoke(new Action(() => notifier.ShowInformation(emoji + " Copied to clipboard!")));

            Clipboard.SetText(emoji);
        }
    }
}
