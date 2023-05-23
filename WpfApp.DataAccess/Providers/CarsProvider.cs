using Microsoft.EntityFrameworkCore;
using WpfApp.Domain;
using Car = WpfApp.Logic.GeneratedEntities.Car;
using DbCar = WpfApp.DataAccess.Entities.Car;

namespace WpfApp.DataAccess.Providers;

public class CarsProvider : IGeneratedEntityProvider<Car>
{
    private readonly DbContextOptions<WpfAppDbContext> _options;

    public CarsProvider(DbContextOptions<WpfAppDbContext> options)
    {
        _options = options;
    }

    public async Task Add(Car car)
    {
        await using var db = new WpfAppDbContext(_options);
        await db.Cars.AddAsync(Map(car));
        await db.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<Car>> GetAll()
    {
        await using var db = new WpfAppDbContext(_options);
        return db.Cars.AsEnumerable().Select(Map).ToList();
    }

    private Car Map(DbCar car) => new Car()
    {
        Id = car.Id,
        Name = car.Name,
        GeneratedDate = car.GeneratedDate
    };

    private DbCar Map(Car car) => new DbCar()
    {
        Name = car.Name,
        GeneratedDate = car.GeneratedDate
    };
}