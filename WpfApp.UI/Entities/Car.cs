using System;
using WpfApp.Logic;

namespace WpfApp.UI.Entities;

public class Car : IGenerationSettings
{
    public string[] AvailableValues { get; } = new[]
    {
        "Мондео", "Крета", "Приус", "УАЗик", "Вольво", "Фокус", "Октавия", "Запорожец",
    };

    public TimeSpan Interval { get; } = TimeSpan.FromSeconds(2);
}

