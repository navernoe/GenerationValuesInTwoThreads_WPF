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

    public async Task<IReadOnlyCollection<(Car, Driver)>> GetAllLinkedEntities(IEnumerable<Car>? entities1 = null, IEnumerable<Driver>? entities2 = null)
    {
        await using var db = new WpfAppDbContext(_options);

        var cars = entities1 ?? db.Cars.AsEnumerable().Select(Map).ToList();
        var drivers = entities2 ?? db.Drivers.AsEnumerable().Select(Map).ToList();;

        var joined = drivers.Join(cars,
            (d) => d.CarId,
            (c) => c.Id,
            (d, c) => (c, d)).ToList();

        return joined;
    }

    private Car Map(Entities.Car car) => new Car()
    {
        Id = car.Id,
        Name = car.Name,
        GeneratedDate = car.GeneratedDate
    };

    private Driver Map(Entities.Driver driver) => new Driver()
    {
        Id = driver.Id,
        Name = driver.Name,
        GeneratedDate = driver.GeneratedDate,
        CarId = driver.CarId
    };
}