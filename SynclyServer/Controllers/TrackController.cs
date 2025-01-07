using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SpotifyApi;
using SynclyServer.Interfaces;
using SynclyServer.Mappers;

namespace SynclyServer.Controllers;

[Route("api/track")]
[ApiController]
public class TrackController(
    ITrackRepository trackRepository,
    IRegistrationRepository registrationRepository)
    : ControllerBase
{
    private readonly ITrackRepository TrackRepository = trackRepository;
    private readonly IRegistrationRepository RegistrationRepository = registrationRepository;

    // [HttpGet]
    // public async Task<IActionResult> GetAll()
    // {
    //     var tracks = await TrackRepository.GetAllAsync();
    //     return Ok(tracks.Select(t => t.ToTrackDto()));
    // }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var track = await TrackRepository.GetByIdAsync(id);
        if (track is null)
            return NotFound();
        return Ok(track.ToTrackDto());
    }

    [HttpGet("current/{userId:int}")]
    public async Task<IActionResult> GetCurrentTrackByUserId([FromRoute] int userId)
    {
        var registration = await RegistrationRepository.GetRegistrationDataByUserIdAsync(userId);
        if (registration is null)
            return NotFound();
        var response = await SpotifyApi.SpotifyApi.GetCurrentTrackAsync(registration.AccessToken);
        if (response.Result is not ApiResult.Success)
            return NotFound();
        var track = ModelsCreator.CreateTrack(response.Data!);
        track.LinksId = 0;
        return Ok(track.ToTrackDto());
    }
}