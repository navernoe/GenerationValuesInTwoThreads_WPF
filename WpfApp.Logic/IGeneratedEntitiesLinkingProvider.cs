namespace WpfApp.Logic;

/// <summary>
/// Провайдер связанный сущностей T1 и T2
/// </summary>
/// <typeparam name="T1">Тип первой сущности</typeparam>
/// <typeparam name="T2">Тип второй сущность, который может быть слинкован с первой</typeparam>
public interface IGeneratedEntitiesLinkingProvider<T1, T2>
     where T1: IGeneratedProperties
     where T2: IGeneratedProperties
{
     /// <summary>
     /// Связать сущности по дате генерации.
     /// </summary>
     /// <param name="generatedDate">Дата генерации.</param>
     /// <returns></returns>
     Task LinkByGeneratedDate(DateTimeOffset generatedDate);

     /// <summary>
     /// Получить все связанные сущности.
     /// </summary>
     /// <param name="entities1">Коллекция сущностей T1 (необязательный параметр, нужен для оптимизации)</param>
     /// <param name="entities2">Коллекция сущностей T2 (необязательный параметр, нужен для оптимизации)</param>
     /// <returns></returns>
     Task<IReadOnlyCollection<(T1, T2)>> GetAllLinkedEntities(IEnumerable<T1>? entities1 = null, IEnumerable<T2>? entities2 = null);
}