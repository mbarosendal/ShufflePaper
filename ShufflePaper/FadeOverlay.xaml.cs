using System.Windows;
using System.Windows.Media.Animation;

namespace ShufflePaper
{
    public partial class FadeOverlay : Window
    {
        public FadeOverlay()
        {
            InitializeComponent();
        }

        public void BeginFadeOut()
        {
            var fadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(1.5)));
            fadeOut.Completed += (s, e) => Close();
            this.BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}
