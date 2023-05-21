using Microsoft.EntityFrameworkCore;
using WpfApp.DataAccess.Entities;

namespace WpfApp.DataAccess;

public class WpfAppDbContext : DbContext
{
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Car> Cars { get; set; }

    public WpfAppDbContext(DbContextOptions<WpfAppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Car>()
            .HasIndex(p => p.Id)
            .IsUnique(true);
        modelBuilder.Entity<Driver>()
            .HasIndex(p => p.Id)
            .IsUnique(true);
    }
}