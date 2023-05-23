using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using WpfApp.DataAccess;
using WpfApp.DataAccess.Providers;
using WpfApp.Domain;
using WpfApp.Logic;
using WpfApp.Logic.GeneratedEntities;
using WpfApp.Logic.GenerationSettings;

namespace WpfApp.UI;

public class Program
{
    [STAThread]
    public static void Main()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(ConfigureAppConfiguration)
            .ConfigureLogging(logging =>
            {
                var logFileName = $"{AppDomain.CurrentDomain.BaseDirectory}/logs/{DateTimeOffset.Now:dd-MM-yyyy_HH.mm}_log.json";
                logging.AddFile(
                    logFileName,
                    fileSizeLimitBytes: 10737418);
            })
            .ConfigureServices((builder, services) =>
            {
            services.AddSingleton<App>();
                services.AddSingleton<MainWindow>();
                services.AddSingleton<HistoryWindow>();
                services.AddSingleton<JobManager<Car>>();
                services.AddSingleton<JobManager<Driver>>();
                services.AddSingleton<IGenerationSettings<Car>, CarGenerationSettings>();
                services.AddSingleton<IGenerationSettings<Driver>, DriverGenerationSettings>();
                services.AddSingleton<DataGenerator<Car>>();
                services.AddSingleton<DataGenerator<Driver>>();
                services.AddSingleton(_ =>
                {
                    var optsDbBuilder = new DbContextOptionsBuilder<WpfAppDbContext>().UseNpgsql(builder.Configuration.GetConnectionString("DbSettingsConnection"));
                    optsDbBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DbSettingsConnection"));

                    return optsDbBuilder.Options;
                });

                services.AddSingleton<IGeneratedEntityProvider<Car>, CarsProvider>();
                services.AddSingleton<IGeneratedEntityProvider<Driver>, DriversProvider>();
                services.AddSingleton<IGeneratedEntitiesLinkingProvider<Car, Driver>, GeneratedEntitiesLinkingProvider>();
            })
            .Build();

        RunMigrations(host);

        var app = host.Services.GetService<App>();
        app?.Run();
    }

    static void RunMigrations(IHost host)
    {
        var scopeFactory = host.Services.GetService<IServiceScopeFactory>();
        using var scope = scopeFactory?.CreateScope();
        var context = scope?.ServiceProvider.GetService<WpfAppDbContext>();
        context?.Database.Migrate();
    }

    static void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder builder)
    {
        var location = Assembly.GetExecutingAssembly().Location;
        var executablePath = Path.GetDirectoryName(location);
        builder.AddJsonFile(new PhysicalFileProvider(executablePath ?? location), "appsettings.json", true, true);
    }
}