using Microsoft.AspNetCore.Mvc;
using SpotifyApi.Models;
using SpotifyApi.Models.Authorization;
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
        var userProfile = await SpotifyApi.SpotifyApi.GetUserProfileAsync(accessTokenDto.AccessToken);
        if (userProfile.Data is null)
            return BadRequest($"Couldn't get user data: {userProfile.Response.StatusCode}");
        var userRegistration = await RegistrationRepository.FindAsync(userProfile.Data.Id!, "spotify");
        if (userRegistration is null)
        {
            var newUser = await CreateNewAccount(accessTokenDto, userProfile.Data);
            return CreatedAtRoute("GetUserById", new { id = newUser.Id },
                newUser.ToUserDto());
        }

        userRegistration =
            await RegistrationRepository.UpdateAsync(userRegistration,
                accessTokenDto.ToRegistration(userProfile.Data.Id!));
        if (userRegistration is null)
            return BadRequest("Something went wrong LOL");
        var registeredUser = await UserRepository.GetByIdAsync(userRegistration.UserId!.Value);
        return CreatedAtRoute("GetUserById", new { id = registeredUser.Id },
            registeredUser.ToUserDto());
    }

    private async Task<User> CreateNewAccount(SpotifyAccessTokenDto accessTokenDto, UserProfile userProfile)
    {
        var registrationModel = accessTokenDto.ToRegistration(userProfile.Id!);
        registrationModel = await RegistrationRepository.CreateAsync(registrationModel);
        var imageUrl = userProfile.Images!.FirstOrDefault()?.Url!;
        imageUrl ??= "";
        var linksModel = ModelsCreator.CreateLinks(imageUrl, userProfile.ExternalUrls!.Spotify!, userProfile.Id!);
        linksModel = await LinksRepository.CreateAsync(linksModel);
        var locationModel = await LocationRepository.CreateAsync(new Location());

        var userModel = ModelsCreator.CreateUser(userProfile, registrationModel, locationModel, linksModel);
        return await UserRepository.CreateAsync(userModel);
    }
}