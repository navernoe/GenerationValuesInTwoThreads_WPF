using System.Collections.Concurrent;
using WpfApp.Domain;
using Car = WpfApp.Logic.GeneratedEntities.Car;

namespace WpfApp.Logic.GeneratedValueHandlers;

public class CarsGeneratedHandler : BaseGeneratedValueHandler
{
    private readonly IGeneratedEntityProvider<Car> _carsProvider;

    public CarsGeneratedHandler(
        IGeneratedEntityProvider<Car> carsProvider,
        ConcurrentBag<IGeneratedProperties> generatedValuesContainer) : base(generatedValuesContainer)
    {
        _carsProvider = carsProvider;
    }

    protected override Task AdditionalHandle(IGeneratedProperties generatedValue)
    {
        return Task.CompletedTask;
    }

    protected override async Task UpdateDbAccordingGeneratedValue(IGeneratedProperties newGeneratedValue)
    {
        await _carsProvider.Add(new Car()
        {
            Name = newGeneratedValue.Name,
            GeneratedDate = newGeneratedValue.GeneratedDate,
        });
    }
}