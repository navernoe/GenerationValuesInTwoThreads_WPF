using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfApp.DataAccess.Entities;
using WpfApp.Logic;
using WpfApp.UI.Models;
using WpfApp.Utils.Extensions;
using Driver = WpfApp.Logic.GeneratedEntities.Driver;
using Car = WpfApp.Logic.GeneratedEntities.Car;


namespace WpfApp.UI.GeneratedValueHandlers;

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

    public ConcurrentBag<DataGridGeneratedRow> FoundMatches { get; } = new();

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
            if (!FoundMatches.Any(m => m.GeneratedDateTime.EqualTillSeconds(match.GeneratedDateTime)))
            {
                FoundMatches.Add(match);
                await _linkingProvider.LinkByGeneratedDate(match.GeneratedDateTime); 
            }
        }
    }
    
    private IEnumerable<DataGridGeneratedRow> FindMatchesByGeneratedDate()
    {
        var matches = GeneratedValuesContainer
            .GroupBy(v => v.GeneratedDate.ToStringTillSeconds())
            .Where(g => g.Count() > 1)
            .Select(g => new DataGridGeneratedRow()
            {
                CarName = g.SingleOrDefault(v => v is Car)?.Name,
                DriverName = g.SingleOrDefault(v => v is Driver)?.Name,
                GeneratedDateTime = g.First().GeneratedDate
            });

        return matches;
    }
}