using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

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

        public string GetRandomImagePath(string folderPath)
        {
            var supported = new[] { ".jpg", ".jpeg", ".png", ".bmp" };
            var files = Directory.GetFiles(folderPath)
                                 .Where(f => supported.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
                                 .ToArray();
            if (files.Length == 0)
                return null;

            return files[new Random().Next(files.Length)];
        }

        public void SetWallpaper(string filePath)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filePath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}
