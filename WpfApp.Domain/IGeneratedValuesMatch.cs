namespace WpfApp.Domain;

public interface IGeneratedValuesMatch<T1, T2>
    where T1: IGeneratedProperties
    where T2: IGeneratedProperties
{
    public T1 Entity1 { get; set; }
    public T2 Entity2 { get; set; }
    public string Key { get; set; }
}