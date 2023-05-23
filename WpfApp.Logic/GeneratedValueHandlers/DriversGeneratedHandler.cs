using System.Collections.Concurrent;
using WpfApp.Domain;
using WpfApp.Utils.Extensions;
using Driver = WpfApp.Logic.GeneratedEntities.Driver;
using Car = WpfApp.Logic.GeneratedEntities.Car;


namespace WpfApp.Logic.GeneratedValueHandlers;

public class DriversGeneratedHandler : BaseGeneratedValueHandler
{
    private readonly IGeneratedEntityProvider<Driver> _driversProvider;
    private readonly IGeneratedEntitiesLinkingProvider<Car, Driver> _linkingProvider;

    public DriversGeneratedHandler(
        IGeneratedEntityProvider<Driver> driversProvider,
        IGeneratedEntitiesLinkingProvider<Car, Driver> linkingProvider,
        ConcurrentBag<IGeneratedProperties> generatedValuesContainer) : base(generatedValuesContainer)
    {
        _driversProvider = driversProvider;
        _linkingProvider = linkingProvider;
    }

    public ConcurrentBag<CarDriverMatch> FoundMatches { get; } = new();

    protected override async Task UpdateDbAccordingGeneratedValue(IGeneratedProperties newGeneratedValue)
    {
        await _driversProvider.Add(new Driver()
        {
            Name = newGeneratedValue.Name,
            GeneratedDate = newGeneratedValue.GeneratedDate
        });
    }

    protected override async Task AdditionalHandle(IGeneratedProperties generatedValue)
    {
        var matches = FindMatchesByGeneratedDate().ToList();

        foreach (var match in matches)
        {
            if (FoundMatches.All(m => m.Key != match.Key))
            {
                FoundMatches.Add(match);
                await _linkingProvider.LinkByGeneratedDate(match.Entity2.GeneratedDate);
            }
        }
    }

    private IEnumerable<CarDriverMatch> FindMatchesByGeneratedDate()
    {
        var matches = GeneratedValuesContainer
            .GroupBy(v => v.GeneratedDate.ToStringTillSeconds())
            .Where(g => g.Count() > 1)
            .Select(g => new CarDriverMatch()
            {
                Entity1 = (Car)g.Single(v => v is Car),
                Entity2 = (Driver)g.Single(v => v is Driver),
                Key = g.Key
            });

        return matches;
    }
}