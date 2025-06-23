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

namespace ShufflePaper
{
    public partial class MainWindow : Window
    {
        private string _selectedFolder;
        private readonly WallpaperService _wallpaperService = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _selectedFolder = dialog.SelectedPath;
                FolderPathText.Text = _selectedFolder;
            }
        }

        private void SetRandomWallpaper_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedFolder))
            {
                System.Windows.MessageBox.Show("Please select a folder first.");
                return;
            }

            var imagePath = _wallpaperService.GetRandomImagePath(_selectedFolder);
            if (imagePath == null)
            {
                System.Windows.MessageBox.Show("No valid images found in folder.");
                return;
            }

            _wallpaperService.SetWallpaper(imagePath);
        }
    }
}