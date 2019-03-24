using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace VideoFritter.About
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : Window
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            Image image = (Image)sender;
            MemoryStream memoryStream = new MemoryStream();
            Properties.Resources.VideoFritterIcon.Save(memoryStream);
            image.Source = BitmapFrame.Create(memoryStream);

        }
    }
}
