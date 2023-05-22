using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Logging;
using WpfApp.Logic;
using WpfApp.Logic.GeneratedEntities;
using WpfApp.UI.Models;

namespace WpfApp.UI;

public partial class HistoryWindow : Window
{
    private readonly IGeneratedEntityProvider<Car> _carsProvider;
    private readonly IGeneratedEntityProvider<Driver> _driversProvider;
    private readonly ILogger<HistoryWindow> _logger;

    public HistoryWindow(
        IGeneratedEntityProvider<Car> carsProvider,
        IGeneratedEntityProvider<Driver> driversProvider,
        ILogger<HistoryWindow> logger)
    {
        InitializeComponent();
        _logger = logger;
        _carsProvider = carsProvider;
        _driversProvider = driversProvider;
        IsVisibleChanged += Window_IsVisibleChanged;
        Closing += Window_OnClosing;
    }

    private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Task.Run(async () =>
        {
            while (Visibility == Visibility.Visible)
            {
                var cars = (await _carsProvider.GetAll()).ToList();
                var drivers = (await _driversProvider.GetAll()).ToList();

                var joined = drivers.Join(cars,
                    (d) => d.CarId,
                    (c) => c.Id,
                    (d, c) => new DataGridGeneratedRow()
                    {
                        CarId = c.Id,
                        DriverId = d.Id,
                        CarName = c.Name,
                        DriverName = d.Name,
                        GeneratedDateTime = d.GeneratedDate
                    }).ToList();
                var joinedCarsIds = joined.Select(j => j.CarId);
                var joinedDriversIds = joined.Select(j => j.DriverId);
                var carsNotJoined = cars.Where(c => !joinedCarsIds.Contains(c.Id)).Select(c =>
                    new DataGridGeneratedRow()
                {
                    CarId = c.Id,
                    CarName = c.Name,
                    DriverName = null,
                    GeneratedDateTime = c.GeneratedDate
                }).ToList();
                var driversNotJoined = drivers.Where(d => !joinedDriversIds.Contains(d.Id)).Select(d =>
                    new DataGridGeneratedRow()
                    {
                        DriverId = d.Id,
                        CarName = null,
                        DriverName = d.Name,
                        GeneratedDateTime = d.GeneratedDate
                    }).ToList();

                var allRows = joined.Concat(carsNotJoined).Concat(driversNotJoined).OrderByDescending(r => r.GeneratedDateTime).ToList();

                Dispatcher.Invoke(() =>
                {
                    HistoryDataGrid.ItemsSource = allRows;
                });

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        });
    }

    private void Window_OnClosing(object? sender, CancelEventArgs e)
    {
        e.Cancel = true;
        Visibility = Visibility.Hidden;
        _logger.LogInformation("History window was closed");
    }
}