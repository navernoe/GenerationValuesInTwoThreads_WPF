using System;
using WpfApp.Logic;

namespace WpfApp.UI.GeneratedEntities;

public class CarGenerationSettings : IGenerationSettings<Car>
{
    public string[] AvailableNameValues { get; } = new[]
    {
        "Мондео", "Крета", "Приус", "УАЗик", "Вольво", "Фокус", "Октавия", "Запорожец",
    };

    public TimeSpan Interval { get; } = TimeSpan.FromSeconds(2);
}

