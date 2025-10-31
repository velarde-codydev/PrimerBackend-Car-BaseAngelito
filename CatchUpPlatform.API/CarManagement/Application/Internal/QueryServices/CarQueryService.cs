using CatchUpPlatform.API.CarManagement.Domain.Model.Aggregates;
using CatchUpPlatform.API.CarManagement.Domain.Model.Queries;
using CatchUpPlatform.API.CarManagement.Domain.Repositories;
using CatchUpPlatform.API.CarManagement.Domain.Services;

namespace CatchUpPlatform.API.CarManagement.Application.Internal.QueryServices;

public class CarQueryService(ICarRepository carRepository) : ICarQueryService
{
    public async Task<Car?> Handle(GetCarByIdQuery query) => await carRepository.FindByIdAsync(query.Id);
    public async Task<IEnumerable<Car>> Handle(GetAllCarsQuery query) => await carRepository.ListAsync(); 
}
