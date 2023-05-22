using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpfApp.DataAccess.Entities;
using WpfApp.DataAccess.Providers;
using WpfApp.Logic;
using WpfApp.UI.Models;
using WpfApp.Utils.Extensions;

namespace WpfApp.UI.GeneratedValueHandlers;

public class DriversGeneratedHandler : BaseGeneratedValueHandler
{
    private readonly DriversProvider _driversProvider;

    public DriversGeneratedHandler(
        DriversProvider driversProvider,
        ConcurrentBag<IGeneratedProperties> generatedValuesContainer) : base(generatedValuesContainer)
    {
        _driversProvider = driversProvider;
    }

    public List<DataGridGeneratedRow> FoundMatches { get; } = new();

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
                await _driversProvider.LinkByGeneratedDate(match.GeneratedDateTime); 
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
                CarName = g.SingleOrDefault(v => v is GeneratedEntities.Car)?.Name,
                DriverName = g.SingleOrDefault(v => v is GeneratedEntities.Driver)?.Name,
                GeneratedDateTime = g.First().GeneratedDate
            });

        return matches;
    }
}