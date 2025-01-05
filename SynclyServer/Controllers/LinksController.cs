using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SynclyServer.Interfaces;
using SynclyServer.Mappers;


namespace SynclyServer.Controllers;

[Route("api/links")]
[ApiController]
public class LinksController(ILinksRepository linksRepository) : ControllerBase
{
    private readonly ILinksRepository LinksRepository = linksRepository;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var links = await LinksRepository.GetAllAsync();
        return Ok(links.Select(l => l.ToLinksDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var userModel = await LinksRepository.GetByIdAsync(id);
        if (userModel is null)
            return NotFound();
        return Ok(userModel.ToLinksDto());
    }
}