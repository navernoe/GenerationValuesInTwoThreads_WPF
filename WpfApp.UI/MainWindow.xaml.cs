using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Logging;
using WpfApp.Logic;
using WpfApp.Logic.GeneratedEntities;
using WpfApp.Utils.Extensions;
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
        private readonly ILogger<MainWindow> _logger;


        private ConcurrentBag<IGeneratedProperties> _generatedValues = new();
        private ConcurrentBag<string> _generatedDatesMatchesAddedToGrid = new();


        public MainWindow(
            HistoryWindow historyWindow,
            DataGenerator<Car> carsGenerator,
            DataGenerator<Driver> driversGenerator,
            IGeneratedEntityProvider<Car> carsProvider,
            IGeneratedEntityProvider<Driver> driversProvider,
            IGeneratedEntitiesLinkingProvider<Car, Driver> linkingProvider,
            ILogger<MainWindow> logger)
        {
            InitializeComponent();
            _logger = logger;
            _historyWindow = historyWindow;
            _carsGenerator = carsGenerator;
            _driversGenerator = driversGenerator;
            _carsHandler = new CarsGeneratedHandler(carsProvider, _generatedValues);
            _driversHandler = new DriversGeneratedHandler(driversProvider, linkingProvider, _generatedValues);
            carsGenerator.GeneratedValues.CollectionChanged += _carsHandler.GeneratedValues_CollectionChanged;
            driversGenerator.GeneratedValues.CollectionChanged += _driversHandler.GeneratedValues_CollectionChanged;
            carsGenerator.GeneratedValues.CollectionChanged += GeneratedValues_CollectionChanged;
            driversGenerator.GeneratedValues.CollectionChanged += GeneratedValues_CollectionChanged;
            Closing += Window_OnClosing;
            carsGenerator.StartGenerate();
            driversGenerator.StartGenerate();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInformation($"Start button was clicked with checked values: cars {CarsCheckbox.IsChecked}, drivers {DriversCheckbox.IsChecked}");

            if (IsChecked(CarsCheckbox))
            {
                _logger.LogInformation("Cars generation was started");

                _carsGenerator.StartGenerate();
            }
            
            if (IsChecked(DriversCheckbox))
            {
                _logger.LogInformation("Drivers generation was started");

                _driversGenerator.StartGenerate();
            }
        }
        
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInformation($"Stop button was clicked with checked values: cars {CarsCheckbox.IsChecked}, drivers {DriversCheckbox.IsChecked}");

            if (IsChecked(CarsCheckbox))
            {
                _logger.LogInformation("Cars generation was stopped");

                _carsGenerator.StopGenerate();
            }
            
            if (IsChecked(DriversCheckbox))
            {
                _logger.LogInformation("Drivers generation was stopped");

                _driversGenerator.StopGenerate();
            }
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInformation("History button was clicked");

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
                            foreach (var match in _driversHandler.FoundMatches)
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
        
        private void Window_OnClosing(object? sender, CancelEventArgs e)
        {
            Visibility = Visibility.Hidden;
            _historyWindow.Visibility = Visibility.Collapsed;
            _logger.LogInformation("Main window was closed");
            _driversGenerator.StopGenerate();
            _carsGenerator.StopGenerate();
            App.Current.Shutdown();
            Process.GetCurrentProcess().Kill();
            base.OnClosed(e);
        }

        private bool IsChecked(CheckBox checkBox)
        {
            var isCarsChecked = checkBox.IsChecked;
            return isCarsChecked.HasValue && isCarsChecked.Value;
        }
    }
}