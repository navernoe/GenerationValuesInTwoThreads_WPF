using System;
using WpfApp.Logic;

namespace WpfApp.UI.GeneratedEntities;

public class DriverGenerationSettings : IGenerationSettings<Driver>
{
    public string[] AvailableNameValues { get; } = new[]
    {
        "Петр", "Василий", "Николай", "Марина", "Феодосий", "Карина"
    };
    
    public TimeSpan Interval { get; } = TimeSpan.FromSeconds(3);

}