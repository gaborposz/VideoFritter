using System.Windows;
using System.Windows.Input;

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

        private void WindowDragging(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
