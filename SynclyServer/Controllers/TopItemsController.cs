using Microsoft.AspNetCore.Mvc;
using SynclyServer.Interfaces;
using SynclyServer.Mappers;

namespace SynclyServer.Controllers;

[Route("api/top")]
[ApiController]
public class TopItemsController(
    ITopTracksRepository topTracksRepository,
    ITopArtistsRepository topArtistsRepository) : ControllerBase
{
    private readonly ITopTracksRepository TopTracksRepository = topTracksRepository;
    private readonly ITopArtistsRepository TopArtistsRepository = topArtistsRepository;

    [HttpGet("track/{userId:int}")]
    public async Task<IActionResult> GetTopTracks([FromRoute] int userId)
    {
        var topTracks = await TopTracksRepository.GetTopTracksAsync(userId);
        if (topTracks is null)
            return NotFound();
        return Ok(topTracks.Select(t => t.ToTrackDto()));
    }

    [HttpGet("artist/{userId:int}")]
    public async Task<IActionResult> GetTopArtists([FromRoute] int userId)
    {
        var topArtists = await TopArtistsRepository.GetTopArtistsAsync(userId);
        if (topArtists is null)
            return NotFound();
        return Ok(topArtists.Select(a => a.ToArtistDto()));
    }
}