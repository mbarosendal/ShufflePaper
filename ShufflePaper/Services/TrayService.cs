using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace ShufflePaper
{
    public class TrayService : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;

        public event EventHandler? ShowRequested;
        public event EventHandler? ExitRequested;

        public TrayService()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Text = "ShufflePaper",
                Visible = true
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Show", null, (_, _) => ShowRequested?.Invoke(this, EventArgs.Empty));
            contextMenu.Items.Add("Exit", null, (_, _) => ExitRequested?.Invoke(this, EventArgs.Empty));

            _notifyIcon.ContextMenuStrip = contextMenu;
            _notifyIcon.DoubleClick += (_, _) => ShowRequested?.Invoke(this, EventArgs.Empty);
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
