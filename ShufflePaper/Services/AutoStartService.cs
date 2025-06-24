using Microsoft.Win32;

namespace ShufflePaper
{
    public static class AutoStartService
    {
        private const string AppName = "ShufflePaper";

        public static void Enable()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            var exePath = Environment.ProcessPath ?? System.Reflection.Assembly.GetExecutingAssembly().Location;
            key?.SetValue(AppName, $"\"{exePath}\"");
        }

        public static void Disable()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            key?.DeleteValue(AppName, false);
        }

        public static bool IsEnabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false);
            return key?.GetValue(AppName) != null;
        }
    }
}
