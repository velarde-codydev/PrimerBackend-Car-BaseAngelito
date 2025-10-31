using CatchUpPlatform.API.CarManagement.Domain.Model.Commands;

namespace CatchUpPlatform.API.CarManagement.Domain.Model.Aggregates;

public partial class Car
{
    public int Id { get; set; }
    public string Model { get; set; }
    public string Color { get; set; }

    protected Car()
    {
        Model = string.Empty;
        Color = string.Empty;
    }

    public Car(string model, string color)
    {
        Model = model;
        Color = color; 
    }

    public Car(CreateCarCommand command)
    {
        Model = command.Model;
        Color = command.Color; 
    }
}