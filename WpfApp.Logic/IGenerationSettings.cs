namespace WpfApp.Logic;

public interface IGenerationSettings
{
     string[] AvailableValues { get; }
     
     TimeSpan Interval { get; }
}