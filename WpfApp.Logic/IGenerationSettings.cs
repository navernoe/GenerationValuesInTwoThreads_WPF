namespace WpfApp.Logic;

/// <summary>
/// Настройки для генерации сущности.
/// </summary>
/// <typeparam name="T">Сущность</typeparam>
public interface IGenerationSettings<T> where T: IGeneratedProperties
{
     /// <summary>
     /// Возможные значения для генерации имени сущности.
     /// </summary>
     string[] AvailableNameValues { get; }
     
     /// <summary>
     /// Интервал генерации сущности.
     /// </summary>
     TimeSpan Interval { get; }
}