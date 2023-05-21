using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfApp.DataAccess.Providers;
using WpfApp.UI.Models;

namespace WpfApp.UI;

public partial class HistoryWindow : Window
{
    private readonly CarsProvider _carsProvider;
    private readonly DriversProvider _driversProvider;

    public HistoryWindow(CarsProvider carsProvider, DriversProvider driversProvider)
    {
        InitializeComponent();
        _carsProvider = carsProvider;
        _driversProvider = driversProvider;
        this.IsVisibleChanged += Window_IsVisibleChanged;
    }

    public void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Task.Run(async () =>
        {
            while (this.Visibility == Visibility.Visible)
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

                this.Dispatcher.Invoke(() =>
                {
                    HistoryDataGrid.ItemsSource = allRows;
                });

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        });
    }
}