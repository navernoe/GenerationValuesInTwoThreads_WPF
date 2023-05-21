using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfApp.DataAccess.Providers;
using WpfApp.Logic;
using WpfApp.Utils.Extensions;
using WpfApp.UI.GeneratedEntities;
using WpfApp.UI.GeneratedValueHandlers;

namespace WpfApp.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HistoryWindow _historyWindow;
        private readonly DataGenerator<Car> _carsGenerator;
        private readonly DataGenerator<Driver> _driversGenerator;
        private readonly CarsGeneratedHandler _carsHandler;
        private readonly DriversGeneratedHandler _driversHandler;

        private ConcurrentBag<IGeneratedProperties> _generatedValues = new();
        private ConcurrentBag<string> _generatedDatesMatchesAddedToGrid = new();

        public MainWindow(
            HistoryWindow historyWindow,
            DataGenerator<Car> carsGenerator,
            DataGenerator<Driver> driversGenerator,
            CarsProvider carsProvider,
            DriversProvider driversProvider)
        {
            InitializeComponent();
            _historyWindow = historyWindow;
            _carsGenerator = carsGenerator;
            _driversGenerator = driversGenerator;
            _carsHandler = new CarsGeneratedHandler(carsProvider, _generatedValues);
            _driversHandler = new DriversGeneratedHandler(driversProvider, _generatedValues);
            carsGenerator.GeneratedValues.CollectionChanged += _carsHandler.GeneratedValues_CollectionChanged;
            driversGenerator.GeneratedValues.CollectionChanged += _driversHandler.GeneratedValues_CollectionChanged;
            carsGenerator.GeneratedValues.CollectionChanged += GeneratedValues_CollectionChanged;
            driversGenerator.GeneratedValues.CollectionChanged += GeneratedValues_CollectionChanged;
            carsGenerator.StartGenerate();
            driversGenerator.StartGenerate();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsChecked(CarsCheckbox))
            {
                _carsGenerator.StartGenerate();
            }
            
            if (IsChecked(DriversCheckbox))
            {
                _driversGenerator.StartGenerate();
            }
        }
        
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsChecked(CarsCheckbox))
            {
                _carsGenerator.StopGenerate();
            }
            
            if (IsChecked(DriversCheckbox))
            {
                _driversGenerator.StopGenerate();
            }
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            _historyWindow.Show();
        }

        private void GeneratedValues_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is not null)
            {
                this.Dispatcher.Invoke(() =>
                {
                    foreach (var newItem in e.NewItems)
                    {
                        CarGenerationThreadId.Text = $"cars generation thread id: {_carsGenerator.ThreadId}";
                        DriverGenerationThreadId.Text = $"drivers generation thread id: {_driversGenerator.ThreadId}";
                        CarHandleThreadId.Text = $"cars handle thread id: {_carsHandler.ThreadId}";
                        DriverHandleThreadId.Text = $"drivers handle thread id: {_driversHandler.ThreadId}";

                        if ((IGeneratedProperties) newItem is Driver)
                        {
                            foreach (var match in _driversHandler.Matches)
                            {
                                var generatedDateMatch = match.GeneratedDateTime.ToStringTillSeconds();
                                if (!_generatedDatesMatchesAddedToGrid.Contains(generatedDateMatch))
                                {
                                    _generatedDatesMatchesAddedToGrid.Add(generatedDateMatch);
                                    MatchedGeneratedValueDataGrid.Items.Add(match);
                                }
                            }
                        }
                    }
                });
            }
        }

        private bool IsChecked(CheckBox checkBox)
        {
            var isCarsChecked = checkBox.IsChecked;
            return isCarsChecked.HasValue && isCarsChecked.Value;
        }
    }
}