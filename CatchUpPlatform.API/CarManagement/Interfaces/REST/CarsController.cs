using CatchUpPlatform.API.CarManagement.Domain.Model.Queries;
using CatchUpPlatform.API.CarManagement.Domain.Services;
using CatchUpPlatform.API.CarManagement.Interfaces.REST.Resources;
using CatchUpPlatform.API.CarManagement.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace CatchUpPlatform.API.CarManagement.Interfaces.REST;

[ApiController]
[Route("/api/v1/[controller]")]
public class CarsController(ICarCommandService cmdSrv, ICarQueryService qrySrv) : ControllerBase {
    [HttpPost]
    public async Task<ActionResult> CreateCar([FromBody] CreateCarResource r) {
        var cmd = CreateCarCommandFromResourceAssembler.ToCommandFromResource(r); 
        var result = await cmdSrv.Handle(cmd);
        if (result == null) return BadRequest();
        return CreatedAtAction(nameof(GetCarById), new {id = result.Id}, CarResourceFromEntityAssembler.ToResurceFromEntity(result));
    }
    [HttpGet("{id}")]
    public async Task<ActionResult> GetCarById(int id) {
        var result = await qrySrv.Handle(new GetCarByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(CarResourceFromEntityAssembler.ToResurceFromEntity(result));
    }
    [HttpGet]
    public async Task<ActionResult> GetAllCars() {
        var result = await qrySrv.Handle(new GetAllCarsQuery());
        var resources = result.Select(CarResourceFromEntityAssembler.ToResurceFromEntity);
        return Ok(resources);
    }
    [HttpPut]
    public async Task<ActionResult> UpdateCar([FromBody] UpdateCarResource r) {
        var cmd = CreateCarCommandFromResourceAssembler.ToUpdateCommandFromResource(r);
        var result = await cmdSrv.Handle(cmd);
        if (result == null) return NotFound();
        return Ok(CarResourceFromEntityAssembler.ToResurceFromEntity(result));
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCar(int id) {
        var deleted = await cmdSrv.HandleDelete(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}