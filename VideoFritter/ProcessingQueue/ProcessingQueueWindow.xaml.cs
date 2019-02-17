using System.Windows;

namespace VideoFritter.ProcessingQueue
{
    /// <summary>
    /// Interaction logic for ProcessingQueue.xaml
    /// </summary>
    public partial class ProcessingQueueWindow : Window
    {
        public ProcessingQueueWindow()
        {
            InitializeComponent();
        }

        private ProcessingQueueViewModel ViewModel
        {
            get
            {
                return (ProcessingQueueViewModel)DataContext;
            }
        }

        private void ClearQueueButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Queue.Clear();
        }

        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Queue.RemoveAt(listBox.SelectedIndex);
        }

        private void StartQueueButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ExportQueue();
        }
    }
}
