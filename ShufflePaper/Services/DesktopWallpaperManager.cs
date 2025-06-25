using System;
using System.Runtime.InteropServices;

namespace ShufflePaper.Services
{
    internal class DesktopWallpaperManager
    {
        [ComImport]
        [Guid("B92B56A9-8B55-4E14-9A89-0199BBB6F93B")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IDesktopWallpaper
        {
            void SetWallpaper([MarshalAs(UnmanagedType.LPWStr)] string monitorID, [MarshalAs(UnmanagedType.LPWStr)] string wallpaper);
            void GetWallpaper([MarshalAs(UnmanagedType.LPWStr)] string monitorID, [MarshalAs(UnmanagedType.LPWStr)] out string wallpaper);
            void GetMonitorDevicePathAt(uint monitorIndex, [MarshalAs(UnmanagedType.LPWStr)] out string monitorID);
            void GetMonitorDevicePathCount(out uint count);
            void GetMonitorRECT([MarshalAs(UnmanagedType.LPWStr)] string monitorID, out RECT displayRect);
            void SetBackgroundColor(uint color);
            void GetBackgroundColor(out uint color);
            void SetPosition(int position);
            void GetPosition(out int position);
            void SetSlideshow(IntPtr items);
            void GetSlideshow(out IntPtr items);
            void SetSlideshowOptions(int options, uint slideshowTick);
            void GetSlideshowOptions(out int options, out uint slideshowTick);
            void AdvanceSlideshow([MarshalAs(UnmanagedType.LPWStr)] string monitorID, int direction);
            void GetStatus(out int state);
            void Enable(bool enable);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left, top, right, bottom;
        }

        [ComImport]
        [Guid("C2CF3110-460E-4FC1-B9D0-8A16D6BEA6C5")]
        private class DesktopWallpaperClass
        {
        }

        private readonly IDesktopWallpaper? _wallpaper;
        private readonly bool _isAvailable;

        public DesktopWallpaperManager()
        {
            try
            {
                _wallpaper = new DesktopWallpaperClass() as IDesktopWallpaper;
                _isAvailable = _wallpaper != null;
            }
            catch (Exception)
            {
                _isAvailable = false;
            }
        }

        public bool IsAvailable => _isAvailable;

        public string GetStatus()
        {
            if (!_isAvailable) return "COM interface failed to initialize";
            if (_wallpaper == null) return "Wallpaper object is null";
            return "COM interface working";
        }

        public int MonitorCount
        {
            get
            {
                if (!_isAvailable || _wallpaper == null)
                {
                    System.Diagnostics.Debug.WriteLine("COM interface not available, returning 1");
                    return 1;
                }
                try
                {
                    _wallpaper.GetMonitorDevicePathCount(out uint count);
                    System.Diagnostics.Debug.WriteLine($"COM interface working, found {count} monitors");
                    return (int)count;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"COM call failed: {ex.Message}, returning 1");
                    return 1;
                }
            }
        }

        public void SetWallpaperForMonitor(uint index, string filePath)
        {
            if (!_isAvailable || _wallpaper == null)
                throw new InvalidOperationException("Desktop wallpaper COM interface is not available");

            _wallpaper.GetMonitorDevicePathAt(index, out string monitorId);
            _wallpaper.SetWallpaper(monitorId, filePath);
        }
    }
}