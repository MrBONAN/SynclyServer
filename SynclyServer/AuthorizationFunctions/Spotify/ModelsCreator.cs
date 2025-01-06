using Infrastructure.API.SpotifyAPI.Models;
using SynclyServer.Models;
using SynclyServer.Models.Authorization;

namespace SynclyServer.Mappers;

public static class ModelsCreator
{
    public static User CreateUser(UserProfile userProfile, Registration registrationModel, Location locationModel, Links linksModel)
    {
        return new User() 
        {
            RegistrationId = registrationModel.Id,
            Name = userProfile.DisplayName!,
            LinksId = linksModel.Id,
            LocationId = locationModel.Id,
        };
    }

    public static Links CreateLinks(string imageLink, string objectPageLink, string objectPlatformId)
    {
        return new Links()
        {
            ExternalImageLink = imageLink,
            ExternalLink = objectPageLink,
            ExternalId = objectPlatformId,
            PlatformName = "spotify"
        };
    }
}