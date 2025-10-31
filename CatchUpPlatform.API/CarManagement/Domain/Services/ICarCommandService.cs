using CatchUpPlatform.API.CarManagement.Domain.Model.Aggregates;
using CatchUpPlatform.API.CarManagement.Domain.Model.Commands;

namespace CatchUpPlatform.API.CarManagement.Domain.Services;

public interface ICarCommandService
{
    Task<Car?> Handle(CreateCarCommand command);
    Task<Car?> Handle(UpdateCarCommand command);
    Task<bool> HandleDelete(int id); 
}