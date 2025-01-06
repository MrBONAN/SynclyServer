using Microsoft.AspNetCore.Mvc;
using SynclyServer.Interfaces;
using SynclyServer.Mappers;

namespace SynclyServer.Controllers;

[Route("api/location")]
[ApiController]
public class LocationController(ILocationRepository locationRepository) : ControllerBase
{
    private ILocationRepository LocationRepository = locationRepository;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var locations = await LocationRepository.GetAllAsync();
        return Ok(locations.Select(l => l.ToLocationDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var locationModel = await LocationRepository.GetByIdAsync(id);
        if (locationModel is null)
            return NotFound();
        return Ok(locationModel.ToLocationDto());
    }
    
}