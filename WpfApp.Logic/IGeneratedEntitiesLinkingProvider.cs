namespace WpfApp.Logic;

public interface IGeneratedEntitiesLinkingProvider<T1, T2>
     where T1: IGeneratedProperties
     where T2: IGeneratedProperties
{
     Task LinkByGeneratedDate(DateTimeOffset generatedDate);
}