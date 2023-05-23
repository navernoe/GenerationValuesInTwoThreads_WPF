using Microsoft.EntityFrameworkCore;
using WpfApp.Domain;
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

    public async Task<IReadOnlyCollection<(Car?, Driver?)>> GetAllLinkedEntities()
    {
        await using var db = new WpfAppDbContext(_options);

        var cars = db.Cars.AsEnumerable().Select(Map).ToList();
        var drivers = db.Drivers.AsEnumerable().Select(Map).ToList();;

        List<(Car?, Driver?)> joined = drivers.Join(cars,
            (d) => d.CarId,
            (c) => c.Id,
            (d, c) => ((Car?)c, (Driver?)d)).ToList();

        var joinedCarsIds = joined.Select(j => j.Item1?.Id).ToList();
        var joinedDriversIds = joined.Select(j => j.Item2?.Id).ToList();
        var carsNotJoined = cars.Where(c => !joinedCarsIds.Contains(c.Id)).Select(c => ((Car?)c, (Driver?)null)).ToList();
        var driversNotJoined = drivers.Where(d => !joinedDriversIds.Contains(d.Id)).Select(d => ((Car?)null, (Driver?)d)).ToList();

        return joined.Concat(carsNotJoined).Concat(driversNotJoined).ToList();
    }

    private Car Map(Entities.Car car) => new ()
    {
        Id = car.Id,
        Name = car.Name,
        GeneratedDate = car.GeneratedDate
    };

    private Driver Map(Entities.Driver driver) => new ()
    {
        Id = driver.Id,
        Name = driver.Name,
        GeneratedDate = driver.GeneratedDate,
        CarId = driver.CarId
    };
}