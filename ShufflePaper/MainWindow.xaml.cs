using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;

namespace ShufflePaper
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly WallpaperService _wallpaperService = new();
        private readonly TimerService _timerService = new();
        private readonly TrayService _trayService = new();
        private string? _selectedFolder;
        private int _intervalSeconds = 60;
        private bool _startWithWindows;
        private bool _startOnAuto;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string? SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                _selectedFolder = value;
                Properties.Settings.Default.FolderPath = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public int IntervalSeconds
        {
            get => _intervalSeconds;
            set
            {
                _intervalSeconds = value;
                Properties.Settings.Default.IntervalSeconds = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public string ToggleTimerButtonText => _timerService.IsRunning ? "Stop Auto" : "Start Auto";

        public bool StartWithWindows
        {
            get => _startWithWindows;
            set
            {
                _startWithWindows = value;
                if (value) AutoStartService.Enable(); else AutoStartService.Disable();
                OnPropertyChanged();
            }
        }

        public bool StartOnAuto
        {
            get => _startOnAuto;
            set
            {
                _startOnAuto = value;
                Properties.Settings.Default.StartOnAuto = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Load persisted values from Settings.settings
            SelectedFolder = Properties.Settings.Default.FolderPath;
            IntervalSeconds = Properties.Settings.Default.IntervalSeconds;
            StartOnAuto = Properties.Settings.Default.StartOnAuto;

            StartWithWindows = AutoStartService.IsEnabled();

            if (Environment.GetCommandLineArgs().Contains("--startup"))
            {
                WindowState = WindowState.Minimized;
            }

            if (StartOnAuto)
            {
                ToggleTimer_Click(null, null);
            }

            _timerService.Tick += (s, e) => SetRandomWallpaper();

            _trayService.ShowRequested += (_, _) =>
            {
                Show();
                WindowState = WindowState.Normal;
                Activate();
            };

            _trayService.ExitRequested += (_, _) =>
            {
                _trayService.Dispose();
                Close();
            };
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            if (name == nameof(_timerService.IsRunning)) OnPropertyChanged(nameof(ToggleTimerButtonText));
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Minimized)
            {
                Hide();
                _trayService.ShowBalloon("ShufflePaper", "Running in system tray");
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _trayService.Dispose();
            base.OnClosing(e);
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedFolder = dialog.SelectedPath;
            }
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SelectedFolder) && Directory.Exists(SelectedFolder))
            {
                Process.Start("explorer.exe", SelectedFolder);
            }
        }

        private void SetWallpaper_Click(object sender, RoutedEventArgs e) => SetRandomWallpaper();

        private void SetRandomWallpaper()
        {
            if (string.IsNullOrWhiteSpace(SelectedFolder)) return;

            var file = _wallpaperService.GetRandomImagePath(SelectedFolder);
            if (!string.IsNullOrEmpty(file))
                _wallpaperService.SetWallpaper(file);
        }

        private void ToggleTimer_Click(object? sender, RoutedEventArgs? e)
        {
            _timerService.Interval = TimeSpan.FromSeconds(IntervalSeconds);
            _timerService.Toggle();
            OnPropertyChanged(nameof(ToggleTimerButtonText));
        }

        private void StartTimer()
        {
            _timerService.Interval = TimeSpan.FromSeconds(IntervalSeconds);
            if (!_timerService.IsRunning)
            {
                _timerService.Toggle();
                OnPropertyChanged(nameof(ToggleTimerButtonText));
            }
        }
    }
}