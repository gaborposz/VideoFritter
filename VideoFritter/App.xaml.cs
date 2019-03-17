using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace VideoFritter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionEventHandler;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            DisplayUnexpectedError(e.Exception);
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            DisplayUnexpectedError(e.Exception);
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            DisplayUnexpectedError(e.Exception);
        }

        private void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            DisplayUnexpectedError(ex);
        }

        internal static void DisplayUnexpectedError(Exception ex)
        {
            if (ex is AggregateException aggregateException && aggregateException.InnerExceptions.Count == 1)
            {
                ex = aggregateException.InnerException;
            }

            MessageBox.Show(ex.ToString(), (string)Application.Current.Resources["ErrorDialogTitle"]);
        }
    }
}
