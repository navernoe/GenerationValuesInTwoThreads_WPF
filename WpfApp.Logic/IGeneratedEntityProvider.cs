namespace WpfApp.Logic;

public interface IGeneratedEntityProvider<T> where T: IGeneratedProperties
{
    Task Add(T entity);

    Task<IReadOnlyCollection<T>> GetAll();
}