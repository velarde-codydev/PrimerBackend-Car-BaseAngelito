using CatchUpPlatform.API.CarManagement.Domain.Model.Aggregates;
using CatchUpPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CatchUpPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        base.OnConfiguring(builder);
        builder.AddCreatedUpdatedInterceptor();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Car>().ToTable("Cars");
        builder.Entity<Car>().HasKey(c => c.Id);
        builder.Entity<Car>().Property(c => c.Id).IsRequired().ValueGeneratedOnAdd(); 
        builder.Entity<Car>().Property(c => c.Model).IsRequired();
        builder.Entity<Car>().Property(c => c.Color).IsRequired();
        
        builder.UseSnakeCaseNamingConvention();

    }
}