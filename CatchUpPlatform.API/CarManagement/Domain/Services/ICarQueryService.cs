using CatchUpPlatform.API.CarManagement.Domain.Model.Aggregates;
using CatchUpPlatform.API.CarManagement.Domain.Model.Queries;

namespace CatchUpPlatform.API.CarManagement.Domain.Services;

public interface ICarQueryService
{
    Task<Car?> Handle(GetCarByIdQuery query);
    Task<IEnumerable<Car>> Handle(GetAllCarsQuery query); 
}