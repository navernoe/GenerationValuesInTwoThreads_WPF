using System.Collections.Concurrent;
using System.Threading.Tasks;
using WpfApp.DataAccess.Entities;
using WpfApp.DataAccess.Providers;
using WpfApp.Logic;

namespace WpfApp.UI.GeneratedValueHandlers;

public class CarsGeneratedHandler : BaseGeneratedValueHandler
{
    private readonly CarsProvider _carsProvider;

    public CarsGeneratedHandler(
        CarsProvider carsProvider,
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