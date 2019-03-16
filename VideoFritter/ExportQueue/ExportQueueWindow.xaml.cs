using System.Windows;

namespace VideoFritter.ExportQueue
{
    /// <summary>
    /// Interaction logic for ExportQueueWindow.xaml
    /// </summary>
    public partial class ExportQueueWindow : Window
    {
        public ExportQueueWindow()
        {
            InitializeComponent();
        }

        private ExportQueueViewModel ViewModel
        {
            get
            {
                return (ExportQueueViewModel)DataContext;
            }
        }

        private void ClearQueueButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ClearQueue();
        }

        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RemoveSelectedItem();
        }

        private void StartQueueButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ExportQueue();
        }
    }
}
