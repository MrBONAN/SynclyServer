using Microsoft.AspNetCore.Mvc;
using SynclyServer.Interfaces;
using SynclyServer.Mappers;

namespace SynclyServer.Controllers;

[Route("api/recently-played")]
[ApiController]
public class RecentlyTracksController(ITrackRepository trackRepository) : ControllerBase
{
    private readonly ITrackRepository TrackRepository = trackRepository;

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetRecentlyTracks([FromRoute] int userId)
    {
        var recentlyTracks = await TrackRepository.GetRecentlyTracksAsync(userId);
        if (recentlyTracks is null)
            return NotFound();
        return Ok(recentlyTracks.Select(t => t.ToTrackDto()));
    }
}