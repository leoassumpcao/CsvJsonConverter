using System;
using System.Windows;

namespace CsvJsonConverter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.ToString());
            e.Handled = true;

            if (MessageBox.Show($"{errorMessage}{Environment.NewLine}{Environment.NewLine}Press OK to handle exception and continue.", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error) != MessageBoxResult.OK)
                this.Shutdown();
        }
    }
}
