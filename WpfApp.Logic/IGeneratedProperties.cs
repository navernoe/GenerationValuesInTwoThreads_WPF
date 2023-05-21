namespace WpfApp.Logic;

/// <summary>
/// Интефейс, описывающий свойства, для которых возможна генерация значений через генератор.
/// Т.е сущность, реализующая этот интерфейс может быть частично сгенерирована.
/// </summary>
public interface IGeneratedProperties
{
    string Name { get; set; }

    DateTimeOffset GeneratedDate { get; set; }
}