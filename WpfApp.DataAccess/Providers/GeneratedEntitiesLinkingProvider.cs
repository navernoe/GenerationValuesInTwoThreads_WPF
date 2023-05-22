using Microsoft.EntityFrameworkCore;
using WpfApp.Logic;
using WpfApp.Utils.Extensions;
using Car = WpfApp.Logic.GeneratedEntities.Car;
using Driver = WpfApp.Logic.GeneratedEntities.Driver;

namespace WpfApp.DataAccess.Providers;

public class GeneratedEntitiesLinkingProvider : IGeneratedEntitiesLinkingProvider<Car, Driver>
{
    private readonly DbContextOptions<WpfAppDbContext> _options;

    public GeneratedEntitiesLinkingProvider(DbContextOptions<WpfAppDbContext> options)
    {
        _options = options;
    }

    public async Task LinkByGeneratedDate(DateTimeOffset generatedDate)
    {
        await using var db = new WpfAppDbContext(_options);
        var driver = db.Drivers.AsEnumerable().Single(d => d.GeneratedDate.EqualTillSeconds(generatedDate));
        var car = db.Cars.AsEnumerable().Single(d => d.GeneratedDate.EqualTillSeconds(generatedDate));
        driver.CarId = car.Id;
        await db.SaveChangesAsync();
    }
}