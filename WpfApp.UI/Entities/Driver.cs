using System;
using WpfApp.Logic;

namespace WpfApp.UI.Entities;

public class Driver : IGenerationSettings
{
    public string[] AvailableValues { get; } = new[]
    {
        "Петр", "Василий", "Николай", "Марина", "Феодосий", "Карина"
    };
    
    public TimeSpan Interval { get; } = TimeSpan.FromSeconds(3);

}