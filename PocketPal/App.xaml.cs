using System.Windows;
using System.Windows.Threading;

namespace PocketPal;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DispatcherUnhandledException += OnDispatcherUnhandledException;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(
            $"Pocket Pal hit an unexpected error and needs to close:\n\n{e.Exception.Message}",
            "Pocket Pal",
            MessageBoxButton.OK,
            MessageBoxImage.Error);

        e.Handled = true;
        Shutdown();
    }
}
