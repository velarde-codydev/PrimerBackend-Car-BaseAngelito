using CatchUpPlatform.API.CarManagement.Domain.Model.Commands;
using CatchUpPlatform.API.CarManagement.Interfaces.REST.Resources;

namespace CatchUpPlatform.API.CarManagement.Interfaces.REST.Transform;

public static class CreateCarCommandFromResourceAssembler
{
    public static CreateCarCommand ToCommandFromResource(CreateCarResource r)
        => new(r.Model, r.Color);
    public static UpdateCarCommand ToUpdateCommandFromResource(UpdateCarResource r)
        => new(r.Id, r.Model, r.Color);
}