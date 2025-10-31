using CatchUpPlatform.API.CarManagement.Domain.Model.Aggregates;
using CatchUpPlatform.API.CarManagement.Domain.Model.Commands;
using CatchUpPlatform.API.CarManagement.Domain.Repositories;
using CatchUpPlatform.API.CarManagement.Domain.Services;
using CatchUpPlatform.API.Shared.Domain.Repositories;

namespace CatchUpPlatform.API.CarManagement.Application.Internal.CommandServices;

public class CarCommandService(ICarRepository carRepository, IUnitOfWork unitOfWork) : ICarCommandService
{
    public async Task<Car?> Handle(CreateCarCommand command)
    {
        var entity = new Car(command);
        await carRepository.AddAsync(entity);
        await unitOfWork.CompleteAsync();
        return entity;
    }
    
    public async Task<Car?> Handle(UpdateCarCommand command) {
        var entity = await carRepository.FindByIdAsync(command.Id);
        if (entity == null) return null;
        entity.Model = command.Model;
        entity.Color = command.Color;
        carRepository.Update(entity);
        await unitOfWork.CompleteAsync();
        return entity;
    }
    public async Task<bool> HandleDelete(int id) {
        var entity = await carRepository.FindByIdAsync(id);
        if (entity == null) return false;
        carRepository.Remove(entity);
        await unitOfWork.CompleteAsync();
        return true;
    }
}