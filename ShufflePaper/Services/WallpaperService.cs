using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static System.Net.WebRequestMethods;

namespace ShufflePaper
{
    public class WallpaperService
    {
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

        public static void SetWallpaperStyle(string style)
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

            switch (style.ToLower())
            {
                case "fill":
                    key?.SetValue("WallpaperStyle", "10");
                    key?.SetValue("TileWallpaper", "0");
                    break;
                case "fit":
                    key?.SetValue("WallpaperStyle", "6");
                    key?.SetValue("TileWallpaper", "0");
                    break;
                case "stretch":
                    key?.SetValue("WallpaperStyle", "2");
                    key?.SetValue("TileWallpaper", "0");
                    break;
                case "tile":
                    key?.SetValue("WallpaperStyle", "0");
                    key?.SetValue("TileWallpaper", "1");
                    break;
                case "center":
                    key?.SetValue("WallpaperStyle", "0");
                    key?.SetValue("TileWallpaper", "0");
                    break;
                case "span":
                    key?.SetValue("WallpaperStyle", "22");
                    key?.SetValue("TileWallpaper", "0");
                    break;
                default:
                    throw new ArgumentException("Unknown wallpaper style: " + style);
            }
        }
    }
}