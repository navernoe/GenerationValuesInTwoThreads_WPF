using Microsoft.EntityFrameworkCore;
using WpfApp.Domain;
using Driver = WpfApp.Logic.GeneratedEntities.Driver;
using DbDriver = WpfApp.DataAccess.Entities.Driver;

namespace WpfApp.DataAccess.Providers;

public class DriversProvider : IGeneratedEntityProvider<Driver>
{
    private readonly DbContextOptions<WpfAppDbContext> _options;

    public DriversProvider(DbContextOptions<WpfAppDbContext> options)
    {
        _options = options;
    }

    public async Task Add(Driver driver)
    {
        await using var db = new WpfAppDbContext(_options);
        await db.Drivers.AddAsync(Map(driver));
        await db.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<Driver>> GetAll()
    {
        await using var db = new WpfAppDbContext(_options);
        return db.Drivers.AsEnumerable().Select(Map).ToList();
    }

    private Driver Map(DbDriver driver) => new Driver()
    {
        Id = driver.Id,
        Name = driver.Name,
        GeneratedDate = driver.GeneratedDate,
        CarId = driver.CarId
    };

    private DbDriver Map(Driver driver) => new DbDriver()
    {
        Name = driver.Name,
        GeneratedDate = driver.GeneratedDate
    };
}