using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SynclyServer.Interfaces;
using SynclyServer.Mappers;

namespace SynclyServer.Controllers;

[Route("api/track")]
[ApiController]
public class TrackController(ITrackRepository trackRepository) : ControllerBase
{
    private readonly ITrackRepository TrackRepository = trackRepository;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tracks = await TrackRepository.GetAllAsync();
        return Ok(tracks.Select(t => t.ToTrackDto()));
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var track = await TrackRepository.GetByIdAsync(id);
        if (track is null)
            return NotFound();
        return Ok(track.ToTrackDto());
    }

}