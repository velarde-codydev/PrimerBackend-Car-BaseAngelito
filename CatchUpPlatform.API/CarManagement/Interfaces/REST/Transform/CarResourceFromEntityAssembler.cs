using CatchUpPlatform.API.CarManagement.Domain.Model.Aggregates;
using CatchUpPlatform.API.CarManagement.Interfaces.REST.Resources;

namespace CatchUpPlatform.API.CarManagement.Interfaces.REST.Transform;

public static class CarResourceFromEntityAssembler
{
    public static CarResource ToResurceFromEntity(Car c) => new CarResource(c.Id, c.Model, c.Color); 
}