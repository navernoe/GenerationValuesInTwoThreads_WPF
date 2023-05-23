using WpfApp.Domain;
using WpfApp.Logic.GeneratedEntities;

namespace WpfApp.Logic.GenerationSettings;

public class CarGenerationSettings : IGenerationSettings<Car>
{
    public string[] AvailableNameValues { get; } = new[]
    {
        "Мондео", "Крета", "Приус", "УАЗик", "Вольво", "Фокус", "Октавия", "Запорожец",
    };

    public TimeSpan Interval { get; init; } = TimeSpan.FromSeconds(2);
}