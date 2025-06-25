using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ShufflePaper.Services;

namespace ShufflePaper
{
    public class WallpaperService
    {
        private readonly DesktopWallpaperManager _manager;

        public WallpaperService()
        {
            _manager = new DesktopWallpaperManager();
        }

        public int ImagesFound { get; private set; }
        public int MonitorCount => _manager.MonitorCount;

        public string? GetRandomImagePath(string folderPath)
        {
            var files = GetImageFiles(folderPath);
            return files.Length == 0 ? null : files[new Random().Next(files.Length)];
        }

        public void UpdateImageCount(string folderPath)
        {
            ImagesFound = GetImageFiles(folderPath).Length;
            DebugMonitorInfo();
        }

        public void DebugMonitorInfo()
        {
            if (_manager != null)
            {
                MessageBox.Show($"COM Status: {_manager.GetStatus()}\n" +
                               $"Monitor Count: {_manager.MonitorCount}\n" +
                               $"System Monitor Count: {System.Windows.Forms.Screen.AllScreens.Length}");
            }
        }

        private static string[] GetImageFiles(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                return Array.Empty<string>();

            var supported = new[] { ".jpg", ".jpeg", ".png", ".bmp" };
            return Directory.GetFiles(folderPath)
                .Where(f => supported.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
                .ToArray();
        }

        public void SetSameWallpaperAll(string filePath)
        {
            SystemParametersInfo(20, 0, filePath, 0x01 | 0x02);
        }

        public void SetRandomPerMonitor(string folderPath)
        {
            var files = GetImageFiles(folderPath);
            if (files.Length == 0)
                return;

            var random = new Random();
            for (uint i = 0; i < _manager.MonitorCount; i++)
            {
                string randomImage = files[random.Next(files.Length)];
                _manager.SetWallpaperForMonitor(i, randomImage);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
    }
}
