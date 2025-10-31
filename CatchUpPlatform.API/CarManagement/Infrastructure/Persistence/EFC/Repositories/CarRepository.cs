using CatchUpPlatform.API.CarManagement.Domain.Model.Aggregates;
using CatchUpPlatform.API.CarManagement.Domain.Repositories;
using CatchUpPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using CatchUpPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace CatchUpPlatform.API.CarManagement.Infrastructure.Persistence.EFC.Repositories;

public class CarRepository : BaseRepository<Car>, ICarRepository
{
    public CarRepository(AppDbContext context) : base(context)
    {
        
    }
}