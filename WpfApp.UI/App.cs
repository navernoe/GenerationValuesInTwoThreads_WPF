using System.Windows;
using Microsoft.Extensions.Logging;

namespace WpfApp.UI;

public class App : Application
{
    readonly MainWindow mainWindow;
    private readonly ILogger<App> _logger;

    public App(MainWindow mainWindow, ILogger<App> logger)
    {
        this.mainWindow = mainWindow;
        ShutdownMode = ShutdownMode.OnMainWindowClose;
        _logger = logger;
    }
    protected override void OnStartup(StartupEventArgs e)
    {
        _logger.LogInformation("Application was started");
        mainWindow.Show();
        base.OnStartup(e);
    }
}