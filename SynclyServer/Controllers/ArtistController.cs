using Microsoft.AspNetCore.Mvc;
using SynclyServer.Interfaces;
using SynclyServer.Mappers;

namespace SynclyServer.Controllers;

[Route("api/artist")]
[ApiController]
public class ArtistController(IArtistRepository artistRepository) : ControllerBase
{
    private readonly IArtistRepository ArtistRepository = artistRepository;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var artists = await ArtistRepository.GetAllAsync();
        return Ok(artists.Select(a => a.ToArtistDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var artist = await ArtistRepository.GetByIdAsync(id);
        if (artist is null)
            return NotFound();
        return Ok(artist.ToArtistDto());
    }
}