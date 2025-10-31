# Proyecto DDD con nombres y estructura clara: Car Management

A continuación te presento la estructura sugerida para un proyecto monolítico siguiendo DDD, con nombres de namespaces y carpetas claros y siguiendo las mejores prácticas. Usaremos el dominio **Car Management**, la entidad principal será **Car**. También encontrarás las secciones completas para `AppDbContext` en Shared y la integración en `Program.cs`.[1][3][5]

## 1. Namespaces y Carpetas 
- **Company.Product.Domain.Model.Aggregates** (por ejemplo, `Acme.CarManagement.Domain.Model.Aggregates`)
- **Company.Product.Domain.Model.Commands**
- **Company.Product.Domain.Model.Queries**
- **Company.Product.Domain.Repositories**
- **Company.Product.Domain.Services**
- **Company.Product.Application.Internal.CommandServices**
- **Company.Product.Application.Internal.QueryServices**
- **Company.Product.Infrastructure.Persistence.Efc.Repositories**
- **Company.Product.Interfaces.Rest.Resources**
- **Company.Product.Interfaces.Rest.Transform**
- **Company.Product.Interfaces.Rest**
- **Company.Product.Shared.***

Esto ayuda a evitar conflictos, es legible y modular.

## 2. Car Aggregate (Dominio)
```csharp
namespace Acme.CarManagement.Domain.Model.Aggregates;

public partial class Car
{
    public int Id { get; set; }
    public string Model { get; set; }
    public string Color { get; set; }

    // Constructor protegido para EF
    protected Car()
    {
        Model = string.Empty;
        Color = string.Empty;
    }
    // Constructor público para lógica de negocio
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
```

## 3. Commands & Queries
```csharp
namespace Acme.CarManagement.Domain.Model.Commands;
public record CreateCarCommand(string Model, string Color);
public record UpdateCarCommand(int Id, string Model, string Color);

namespace Acme.CarManagement.Domain.Model.Queries;
public record GetAllCarsQuery();
public record GetCarByIdQuery(int Id);
```

## 4. Repositorios y Servicios (Interfaces)
```csharp
using Acme.CarManagement.Domain.Model.Aggregates;
using Acme.CarManagement.Domain.Model.Commands;
using Acme.CarManagement.Domain.Model.Queries;
using Acme.CarManagement.Shared.Domain.Repositories;

namespace Acme.CarManagement.Domain.Repositories;
public interface ICarRepository : IBaseRepository<Car> { }

namespace Acme.CarManagement.Domain.Services;
public interface ICarCommandService {
    Task<Car?> Handle(CreateCarCommand command);
    Task<Car?> Handle(UpdateCarCommand command);
    Task<bool> HandleDelete(int id);
}
public interface ICarQueryService {
    Task<Car?> Handle(GetCarByIdQuery query);
    Task<IEnumerable<Car>> Handle(GetAllCarsQuery query);
}
```

## 5. Infraestructura
```csharp
using Acme.CarManagement.Domain.Model.Aggregates;
using Acme.CarManagement.Domain.Repositories;
using Acme.CarManagement.Shared.Infrastructure.Persistence.Efc.Configuration;
using Acme.CarManagement.Shared.Infrastructure.Persistence.Efc.Repositories;

namespace Acme.CarManagement.Infrastructure.Persistence.Efc.Repositories;
public class CarRepository : BaseRepository<Car>, ICarRepository {
    public CarRepository(AppDbContext context) : base(context) { }
}
```

## 6. Aplicación
```csharp
using Acme.CarManagement.Domain.Model.Aggregates;
using Acme.CarManagement.Domain.Model.Commands;
using Acme.CarManagement.Domain.Repositories;
using Acme.CarManagement.Domain.Services;
using Acme.CarManagement.Shared.Domain.Repositories;

namespace Acme.CarManagement.Application.Internal.CommandServices;
public class CarCommandService(ICarRepository carRepo, IUnitOfWork unitOfWork) : ICarCommandService {
    public async Task<Car?> Handle(CreateCarCommand command) {
        var entity = new Car(command);
        await carRepo.AddAsync(entity);
        await unitOfWork.CompleteAsync();
        return entity;
    }
    public async Task<Car?> Handle(UpdateCarCommand command) {
        var entity = await carRepo.FindByIdAsync(command.Id);
        if (entity == null) return null;
        entity.Model = command.Model;
        entity.Color = command.Color;
        carRepo.Update(entity);
        await unitOfWork.CompleteAsync();
        return entity;
    }
    public async Task<bool> HandleDelete(int id) {
        var entity = await carRepo.FindByIdAsync(id);
        if (entity == null) return false;
        carRepo.Remove(entity);
        await unitOfWork.CompleteAsync();
        return true;
    }
}

using Acme.CarManagement.Domain.Model.Queries;
namespace Acme.CarManagement.Application.Internal.QueryServices;
public class CarQueryService(ICarRepository carRepo) : ICarQueryService {
    public async Task<Car?> Handle(GetCarByIdQuery query) => await carRepo.FindByIdAsync(query.Id);
    public async Task<IEnumerable<Car>> Handle(GetAllCarsQuery query) => await carRepo.ListAsync();
}
```

## 7. Interfaces REST: Resources y Controlador
```csharp
namespace Acme.CarManagement.Interfaces.Rest.Resources;
public record CreateCarResource(string Model, string Color);
public record UpdateCarResource(int Id, string Model, string Color);
public record CarResource(int Id, string Model, string Color);

using Acme.CarManagement.Domain.Model.Aggregates;
namespace Acme.CarManagement.Interfaces.Rest.Transform;
public static class CarResourceFromEntityAssembler {
    public static CarResource ToResourceFromEntity(Car c) => new CarResource(c.Id, c.Model, c.Color);
}

using Acme.CarManagement.Domain.Model.Commands;
namespace Acme.CarManagement.Interfaces.Rest.Transform;
public static class CreateCarCommandFromResourceAssembler {
    public static CreateCarCommand ToCommandFromResource(CreateCarResource r)
        => new(r.Model, r.Color);
    public static UpdateCarCommand ToUpdateCommandFromResource(UpdateCarResource r)
        => new(r.Id, r.Model, r.Color);
}

using Acme.CarManagement.Domain.Services;
using Acme.CarManagement.Interfaces.Rest.Resources;
using Acme.CarManagement.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace Acme.CarManagement.Interfaces.Rest;
[ApiController]
[Route("/api/v1/[controller]")]
public class CarsController(ICarCommandService cmdSrv, ICarQueryService qrySrv) : ControllerBase {
    [HttpPost]
    public async Task<ActionResult> CreateCar([FromBody] CreateCarResource r) {
        var cmd = CreateCarCommandFromResourceAssembler.ToCommandFromResource(r);
        var result = await cmdSrv.Handle(cmd);
        if (result == null) return BadRequest();
        return CreatedAtAction(nameof(GetCarById), new {id = result.Id}, CarResourceFromEntityAssembler.ToResourceFromEntity(result));
    }
    [HttpGet("{id}")]
    public async Task<ActionResult> GetCarById(int id) {
        var result = await qrySrv.Handle(new GetCarByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(CarResourceFromEntityAssembler.ToResourceFromEntity(result));
    }
    [HttpGet]
    public async Task<ActionResult> GetAllCars() {
        var result = await qrySrv.Handle(new GetAllCarsQuery());
        var resources = result.Select(CarResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
    [HttpPut]
    public async Task<ActionResult> UpdateCar([FromBody] UpdateCarResource r) {
        var cmd = CreateCarCommandFromResourceAssembler.ToUpdateCommandFromResource(r);
        var result = await cmdSrv.Handle(cmd);
        if (result == null) return NotFound();
        return Ok(CarResourceFromEntityAssembler.ToResourceFromEntity(result));
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCar(int id) {
        var deleted = await cmdSrv.HandleDelete(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
```

## 8. AppDbContext en Shared
```csharp
using Acme.CarManagement.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Acme.CarManagement.Shared.Infrastructure.Persistence.Efc.Configuration;
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Car>().ToTable("Cars");
        builder.Entity<Car>().HasKey(c => c.Id);
        builder.Entity<Car>().Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Car>().Property(c => c.Model).IsRequired();
        builder.Entity<Car>().Property(c => c.Color).IsRequired();
        // Cualquier otra convención
    }
}
```

## 9. Program.cs para inyectar dependencia
```csharp
using Acme.CarManagement.Application.Internal.CommandServices;
using Acme.CarManagement.Application.Internal.QueryServices;
using Acme.CarManagement.Domain.Repositories;
using Acme.CarManagement.Domain.Services;
using Acme.CarManagement.Infrastructure.Persistence.Efc.Repositories;
using Acme.CarManagement.Shared.Domain.Repositories;
using Acme.CarManagement.Shared.Infrastructure.Persistence.Efc.Configuration;
using Acme.CarManagement.Shared.Infrastructure.Persistence.Efc.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (connectionString != null)
        options.UseMySQL(connectionString);
});

// Dependency Injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<ICarCommandService, CarCommandService>();
builder.Services.AddScoped<ICarQueryService, CarQueryService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

## Consejos finales
- Usa nombres consistentes, claros y acordes a las guías oficiales para namespaces y clases.
- Usa `plural` para namespaces de colecciones o aggregations, y pascal case siempre.
- Unifica nombres de carpeta y de namespace para evitar problemas de referencia.

¿Te gustaría practicar cómo adaptar este patrón para otro dominio, o te ayudo a armar los archivos del proyecto uno a uno? Cuando lo implementes, revisa cada carpeta, namespace y clase para asegurarte de que todo es consistente y único.

[1](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/names-of-namespaces)
[2](https://stackoverflow.com/questions/918894/namespace-naming-conventions)
[3](https://dev.to/teshanecrawford/demystifying-namespace-naming-in-c-and-net-2d2n)
[4](https://www.reddit.com/r/csharp/comments/1d9eise/what_is_best_practice_regarding_namespaces/)
[5](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names)
[6](http://c-style-guide.readthedocs.io/en/latest/namingconventions.html)
[7](https://csharpcodingguidelines.com/naming-guidelines/)
[8](https://google.github.io/styleguide/csharp-style.html)
