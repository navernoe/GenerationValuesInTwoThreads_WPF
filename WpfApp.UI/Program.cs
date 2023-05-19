using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using WpfApp.Logic;
using WpfApp.UI.Entities;

namespace WpfApp.UI;

public class Program
{
    [STAThread]
    public static void Main()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<App>();
                services.AddSingleton<MainWindow>();
                services.AddSingleton<Car>();
                services.AddSingleton<Driver>();
                services.AddSingleton<JobManager<Car>>();
                services.AddSingleton<JobManager<Driver>>();
                services.AddSingleton<DataGenerator<Car>>();
                services.AddSingleton<DataGenerator<Driver>>();
            })
            .Build();

        var app = host.Services.GetService<App>();
        app?.Run();
    }
    
}