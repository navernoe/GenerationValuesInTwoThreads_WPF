using Microsoft.EntityFrameworkCore;
using WpfApp.DataAccess.Entities;

namespace WpfApp.DataAccess.Providers;

public class CarsProvider
{
    private readonly DbContextOptions<WpfAppDbContext> _options;

    public CarsProvider(DbContextOptions<WpfAppDbContext> options)
    {
        _options = options;
    }

    public async Task Add(Car car)
    {
        await using var db = new WpfAppDbContext(_options);
        await db.Cars.AddAsync(car);
        await db.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<Car>> GetAll()
    {
        await using var db = new WpfAppDbContext(_options);
        return db.Cars.AsEnumerable().ToList();
    }
}