using System;
using WpfApp.Logic;
using WpfApp.Logic.GeneratedEntities;

namespace WpfApp.UI.GenerationSettings;

public class CarGenerationSettings : IGenerationSettings<Car>
{
    public string[] AvailableNameValues { get; } = new[]
    {
        "Мондео", "Крета", "Приус", "УАЗик", "Вольво", "Фокус", "Октавия", "Запорожец",
    };

    public TimeSpan Interval { get; init; } = TimeSpan.FromSeconds(2);
}

