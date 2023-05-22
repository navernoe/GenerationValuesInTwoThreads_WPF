namespace WpfApp.Logic;

/// <summary>
/// Провайдер бд для генерируемых сущносьей.
/// </summary>
/// <typeparam name="T">Тип генерируемой сущности</typeparam>
public interface IGeneratedEntityProvider<T> where T: IGeneratedProperties
{
    Task Add(T entity);

    Task<IReadOnlyCollection<T>> GetAll();
}