using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
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
using WpfApp.Logic;
using WpfApp.UI.Entities;

namespace WpfApp.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DataGenerator<Car> _carsGenerator;
        private readonly DataGenerator<Driver> _driversGenerator;

        public MainWindow(DataGenerator<Car> carsGenerator, DataGenerator<Driver> driversGenerator)
        {
            InitializeComponent();
            _carsGenerator = carsGenerator;
            _driversGenerator = driversGenerator;
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

        private void GeneratedValues_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is not null)
            {
                foreach (var newItem in e.NewItems)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        var generatedValue = (GeneratedValue) newItem;
                        GeneratedValueDataGrid.Items.Add(generatedValue);
                    });
                }
            }
        }

        private bool IsChecked(CheckBox checkBox)
        {
            var isCarsChecked = checkBox.IsChecked;
            return isCarsChecked.HasValue && isCarsChecked.Value;
        }
    }
}