using Microsoft.EntityFrameworkCore;
using WpfApp.DataAccess.Entities;
using WpfApp.Utils.Extensions;

namespace WpfApp.DataAccess.Providers;

public class DriversProvider
{
    private readonly DbContextOptions<WpfAppDbContext> _options;

    public DriversProvider(DbContextOptions<WpfAppDbContext> options)
    {
        _options = options;
    }

    public async Task Add(Driver driver)
    {
        await using var db = new WpfAppDbContext(_options);
        await db.Drivers.AddAsync(driver);
        await db.SaveChangesAsync();
    }

    public async Task LinkByGeneratedDate(DateTimeOffset generatedDate)
    {
        await using var db = new WpfAppDbContext(_options);
        var driver = db.Drivers.AsEnumerable().Single(d => d.GeneratedDate.EqualTillSeconds(generatedDate));
        var car = db.Cars.AsEnumerable().Single(d => d.GeneratedDate.EqualTillSeconds(generatedDate));
        driver.CarId = car.Id;
        await db.SaveChangesAsync();
    }
    
    public async Task<IReadOnlyCollection<Driver>> GetAll()
    {
        await using var db = new WpfAppDbContext(_options);
        return db.Drivers.AsEnumerable().ToList();
    }
}