using System.Collections.Concurrent;
using System.Collections.Specialized;

namespace WpfApp.Logic;

public abstract class BaseGeneratedValueHandler
{
    protected ConcurrentBag<IGeneratedProperties> GeneratedValuesContainer;

    protected BaseGeneratedValueHandler(ConcurrentBag<IGeneratedProperties> generatedValuesContainer)
    {
        GeneratedValuesContainer = generatedValuesContainer;
    }

    public int? ThreadId { get; private set; }
    
    public void GeneratedValues_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // запускаем обработку в другом потоке, чтобы она не занимала время потока, который генерирует новые значения.
        Task.Run(async () =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is not null)
            {
                ThreadId = Thread.CurrentThread.ManagedThreadId;

                foreach (var newItem in e.NewItems)
                {
                    var generatedValue = (IGeneratedProperties) newItem;
                    GeneratedValuesContainer.Add(generatedValue);

                    await UpdateDbAccordingGeneratedValue(generatedValue);
                    await AdditionalHandle(generatedValue);
                }
            }
        });
    }

    protected abstract Task AdditionalHandle(IGeneratedProperties generatedValue);

    protected abstract Task UpdateDbAccordingGeneratedValue(IGeneratedProperties newGeneratedValue);
}