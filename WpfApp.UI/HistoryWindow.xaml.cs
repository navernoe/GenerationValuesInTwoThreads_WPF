using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Logging;
using WpfApp.Domain;
using WpfApp.Logic.GeneratedEntities;
using WpfApp.UI.Models;

namespace WpfApp.UI;

public partial class HistoryWindow : Window
{
    private readonly IGeneratedEntitiesLinkingProvider<Car, Driver> _linkingProvider;
    private readonly ILogger<HistoryWindow> _logger;

    public HistoryWindow(
        IGeneratedEntitiesLinkingProvider<Car, Driver> linkingProvider,
        ILogger<HistoryWindow> logger)
    {
        InitializeComponent();
        _logger = logger;
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
        var allLinkedEntities = await _linkingProvider.GetAllLinkedEntities();

        return allLinkedEntities.Select(Map).OrderByDescending(r => r.GeneratedDateTime).ToList();
    }

    private DataGridGeneratedRow Map((Car?, Driver?) linkedEntities) => new ()
    {
        CarName = linkedEntities.Item1?.Name,
        DriverName = linkedEntities.Item2?.Name,
        GeneratedDateTime = linkedEntities.Item2?.GeneratedDate
            ?? linkedEntities.Item1?.GeneratedDate
            ?? throw new InvalidOperationException("Ни у одной сущности нет даты генерации")
    };

    private void Window_OnClosing(object? sender, CancelEventArgs e)
    {
        e.Cancel = true;
        Visibility = Visibility.Hidden;
        _logger.LogInformation("History window was closed");
    }
}