using System;
using System.Collections.Generic;
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
    private readonly IGeneratedEntitiesLinkingProvider<Car, Driver> _linkingProvider;
    private readonly ILogger<HistoryWindow> _logger;

    public HistoryWindow(
        IGeneratedEntityProvider<Car> carsProvider,
        IGeneratedEntityProvider<Driver> driversProvider,
        IGeneratedEntitiesLinkingProvider<Car, Driver> linkingProvider,
        ILogger<HistoryWindow> logger)
    {
        InitializeComponent();
        _logger = logger;
        _carsProvider = carsProvider;
        _driversProvider = driversProvider;
        _linkingProvider = linkingProvider;
        IsVisibleChanged += Window_IsVisibleChanged;
        Closing += Window_OnClosing;
    }

    private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Task.Run(async () =>
        {
            while (Visibility == Visibility.Visible)
            {
                var allRows = await GetAllGeneratedValuesForHistoryDataGrid();

                Dispatcher.Invoke(() =>
                {
                    HistoryDataGrid.ItemsSource = allRows;
                });

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        });
    }

    private async Task<List<DataGridGeneratedRow>> GetAllGeneratedValuesForHistoryDataGrid()
    {
        var cars = (await _carsProvider.GetAll()).ToList();
        var drivers = (await _driversProvider.GetAll()).ToList();
        var joined = (await _linkingProvider.GetAllLinkedEntities(cars, drivers))
            .Select(t => new DataGridGeneratedRow()
            {
                CarId = t.Item1.Id,
                DriverId = t.Item2.Id,
                CarName = t.Item1.Name,
                DriverName = t.Item2.Name,
                GeneratedDateTime = t.Item2.GeneratedDate
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

        return joined.Concat(carsNotJoined).Concat(driversNotJoined).OrderByDescending(r => r.GeneratedDateTime).ToList();
    }

    private void Window_OnClosing(object? sender, CancelEventArgs e)
    {
        e.Cancel = true;
        Visibility = Visibility.Hidden;
        _logger.LogInformation("History window was closed");
    }
}