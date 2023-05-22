using System;
using WpfApp.Logic;
using WpfApp.Logic.GeneratedEntities;

namespace WpfApp.UI.GenerationSettings;

public class DriverGenerationSettings : IGenerationSettings<Driver>
{
    public string[] AvailableNameValues { get; } = new[]
    {
        "Петр", "Василий", "Николай", "Марина", "Феодосий", "Карина"
    };
    
    public TimeSpan Interval { get; init; } = TimeSpan.FromSeconds(3);

}