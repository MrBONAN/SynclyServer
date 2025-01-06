using Microsoft.AspNetCore.Mvc;
using SynclyServer.Interfaces;
using SynclyServer.Mappers;

namespace SynclyServer.Controllers;

[Route("api/top")]
[ApiController]
public class TopItemsController(IUserRepository userRepository) : ControllerBase
{
    private readonly IUserRepository UserRepository = userRepository;

    [HttpGet("track/{userId:int}")]
    public async Task<IActionResult> GetTopTracks([FromRoute] int userId)
    {
        var topTracks = await UserRepository.GetTopTracksAsync(userId);
        if (topTracks is null)
            return NotFound();
        return Ok(topTracks.Select(t => t.ToTrackDto()));
    }
    
    [HttpGet("artist/{userId:int}")]
    public async Task<IActionResult> GetTopArtists([FromRoute] int userId)
    {
        var topArtists = await UserRepository.GetTopArtistsAsync(userId);
        if (topArtists is null)
            return NotFound();
        return Ok(topArtists.Select(a => a.ToArtistDto()));
    }
}