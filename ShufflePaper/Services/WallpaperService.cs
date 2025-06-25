using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static System.Net.WebRequestMethods;

namespace ShufflePaper
{
    public class WallpaperService
    {
        // DllImport lets you call native Windows functions from C#.
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;
        public int ImagesFound { get; private set; }

        public string? GetRandomImagePath(string folderPath)
        {
            string[] files = GetImageFiles(folderPath);

            if (files.Length == 0)
                return null;

            return files[new Random().Next(files.Length)];
        }

        public void UpdateImageCount(string folderPath)
        {
            string[] files = GetImageFiles(folderPath);

            ImagesFound = files.Length;
        }

        private static string[] GetImageFiles(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                return Array.Empty<string>();

            var supported = new[] { ".jpg", ".jpeg", ".png", ".bmp" };
            var files = Directory.GetFiles(folderPath)
                                 .Where(f => supported.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
                                 .ToArray();
            return files;
        }

        public void SetAsWallpaper(string filePath)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filePath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}