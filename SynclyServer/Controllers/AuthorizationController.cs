using Infrastructure.API.SpotifyAPI;
using Microsoft.AspNetCore.Mvc;
using SynclyServer.Dtos.Authorization.Spotify;
using SynclyServer.Interfaces;
using SynclyServer.Mappers;

namespace SynclyServer.Controllers;

[Route("api/authorize")]
[ApiController]
public class AuthorizationController(IRegistrationRepository registrationRepository) : ControllerBase
{
    private readonly IRegistrationRepository RegistrationRepository = registrationRepository;

    [HttpPost]
    [Route("spotify")]
    public async Task<IActionResult> Authorize([FromBody] SpotifyAccessTokenDto accessTokenDto)
    {
        var userProfile = await SpotifyApi.GetUserProfileAsync(accessTokenDto.AccessToken);
        if (userProfile.Data is null)
            return BadRequest($"Couldn't get user data: {userProfile.Response.StatusCode}");
        var userRegistration = await RegistrationRepository.FindAsync(userProfile.Data.Id!, "spotify");
        if (userRegistration is null)
            throw new NotImplementedException();
        userRegistration = await RegistrationRepository.UpdateAsync(userRegistration, accessTokenDto.ToRegistration());
        
    }
}