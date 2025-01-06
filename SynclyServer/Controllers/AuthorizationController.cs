using Infrastructure.API.SpotifyAPI;
using Infrastructure.API.SpotifyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using SynclyServer.Dtos.Authorization.Spotify;
using SynclyServer.Interfaces;
using SynclyServer.Mappers;
using SynclyServer.Models;

namespace SynclyServer.Controllers;

[Route("api/authorize")]
[ApiController]
public class AuthorizationController(
    IRegistrationRepository registrationRepository,
    IUserRepository userRepository,
    ILinksRepository linksRepository,
    ILocationRepository locationRepository) : ControllerBase
{
    private readonly IRegistrationRepository RegistrationRepository = registrationRepository;
    private readonly IUserRepository UserRepository = userRepository;
    private readonly ILinksRepository LinksRepository = linksRepository;
    private readonly ILocationRepository LocationRepository = locationRepository;


    [HttpPost]
    [Route("spotify")]
    public async Task<IActionResult> Authorize([FromBody] SpotifyAccessTokenDto accessTokenDto)
    {
        var userProfile = await SpotifyApi.GetUserProfileAsync(accessTokenDto.AccessToken);
        if (userProfile.Data is null)
            return BadRequest($"Couldn't get user data: {userProfile.Response.StatusCode}");
        var userRegistration = await RegistrationRepository.FindAsync(userProfile.Data.Id!, "spotify");
        if (userRegistration is null)
        {
            var newUser = await CreateNewAccount(accessTokenDto, userProfile.Data);
            return CreatedAtAction(nameof(UserController.GetById), new { id = newUser.Id },
                newUser.ToUserDto());
        }

        userRegistration = await RegistrationRepository.UpdateAsync(userRegistration, accessTokenDto.ToRegistration());
        return CreatedAtAction(nameof(UserController.GetById), new { id = userRegistration.UserId },
            userRegistration.User.ToUserDto());
    }

    private async Task<User> CreateNewAccount(SpotifyAccessTokenDto accessTokenDto, UserProfile userProfile)
    {
        var registrationModel = accessTokenDto.ToRegistration(userProfile.Id!);
        registrationModel = await RegistrationRepository.CreateAsync(registrationModel);
        
        var linksModel = ModelsCreator.CreateLinks(userProfile.Images!.First().Url!, userProfile.Uri!, userProfile.Id!);
        linksModel = await LinksRepository.CreateAsync(linksModel);
        var locationModel = await LocationRepository.CreateAsync(new Location());
        
        var userModel = ModelsCreator.CreateUser(userProfile, registrationModel, locationModel, linksModel);
        return await UserRepository.CreateAsync(userModel);
    }
}