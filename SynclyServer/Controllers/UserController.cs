using Microsoft.AspNetCore.Mvc;
using SynclyServer.Interfaces;
using SynclyServer.Mappers;

namespace SynclyServer.Controllers;

[Route("api/user")]
[ApiController]
public class UserController(IUserRepository userRepository) : ControllerBase
{
    private readonly IUserRepository UserRepository = userRepository;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await UserRepository.GetAllAsync();
        return Ok(users.Select(u => u.ToUserDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var userModel = await UserRepository.GetByIdAsync(id);
        if (userModel is null)
            return NotFound();
        return Ok(userModel.ToUserDto());
    }
}