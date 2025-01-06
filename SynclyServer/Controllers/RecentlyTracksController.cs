using Microsoft.AspNetCore.Mvc;
using SynclyServer.Interfaces;
using SynclyServer.Mappers;

namespace SynclyServer.Controllers;

[Route("api/recently-played")]
[ApiController]
public class RecentlyTracksController(IUserRepository userRepository) : ControllerBase
{
    private readonly IUserRepository UserRepository = userRepository;

    [HttpGet("recently-played/{userId:int}")]
    public async Task<IActionResult> GetRecentlyTracks([FromRoute] int userId)
    {
        var recentlyTracks = await UserRepository.GetRecentlyTracksAsync(userId);
        if (recentlyTracks is null)
            return NotFound();
        return Ok(recentlyTracks.Select(t => t.ToTrackDto()));
    }
}