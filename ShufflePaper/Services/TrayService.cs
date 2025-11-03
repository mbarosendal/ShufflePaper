using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Reflection;

namespace ShufflePaper
{
    public class TrayService : IDisposable
    {
        public readonly NotifyIcon _notifyIcon;

        public event EventHandler? ShowRequested;
        public event EventHandler? ExitRequested;

        public event EventHandler? LeftClickRequested;

        public TrayService()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("ShufflePaper.Assets.app.ico")),
                Text = "ShufflePaper",
                Visible = true
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Show", null, (_, _) => ShowRequested?.Invoke(this, EventArgs.Empty));
            contextMenu.Items.Add("Exit", null, (_, _) => ExitRequested?.Invoke(this, EventArgs.Empty));

            _notifyIcon.ContextMenuStrip = contextMenu;

            // When left-clicking tray icon, anything subscribed to this happens (currently new random wallpaper via MainWindow contructor)
            _notifyIcon.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                    LeftClickRequested?.Invoke(this, EventArgs.Empty);
            };

            // Standard double click to show program disabled (show via right click instead)
            //_notifyIcon.DoubleClick += (_, _) => ShowRequested?.Invoke(this, EventArgs.Empty);
        }


        public void ShowBalloon(string title, string message, ToolTipIcon icon = ToolTipIcon.Info)
        {
            _notifyIcon.BalloonTipTitle = title;
            _notifyIcon.BalloonTipText = message;
            _notifyIcon.BalloonTipIcon = icon;
            _notifyIcon.ShowBalloonTip(3000);
        }

        public void Dispose()
        {
            _notifyIcon.Dispose();
        }

    }
}
