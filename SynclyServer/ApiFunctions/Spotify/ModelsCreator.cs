using SpotifyApi.Models;
using SynclyServer.Models;
using SynclyServer.Models.Authorization;
using SynclyServer.Repository;
using Artist = SynclyServer.Models.Artist;
using Track = SynclyServer.Models.Track;

namespace SynclyServer.Mappers;

public static class ModelsCreator
{
    public static User CreateUser(UserProfile userProfile, Registration registrationModel, Location locationModel, Links linksModel)
    {
        return new User() 
        {
            Registration = registrationModel,
            Name = userProfile.DisplayName!,
            Location = locationModel,
            Links = linksModel
            // RegistrationId = registrationModel.Id,
            // LinksId = linksModel.Id,
            // LocationId = locationModel.Id,
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

    public static Track CreateTrack(SpotifyApi.Models.Track spotifyTrack)
    {
        var imageUrl = spotifyTrack.Images!.FirstOrDefault()?.Url!;
        imageUrl ??= "";
        var linksModel = new Links()
        {
            ExternalImageLink = imageUrl,
            ExternalLink = spotifyTrack.ExternalUrls!.Spotify!,
            ExternalId = spotifyTrack.Id!,
            PlatformName = "spotify"
        };
        var trackModel = new Track()
        {
            Name = spotifyTrack.Name!,
            //Genre = null,
            Links = linksModel
        };
        return trackModel;
    }
    
    public static Artist CreateArtist(SpotifyApi.Models.Artist spotifyArtist)
    {
        var imageUrl = spotifyArtist.Images!.FirstOrDefault()?.Url!;
        imageUrl ??= "";
        var linksModel = new Links()
        {
            ExternalImageLink = imageUrl,
            ExternalLink = spotifyArtist.ExternalUrls!.Spotify!,
            ExternalId = spotifyArtist.Id!,
            PlatformName = "spotify"
        };
        var artistModel = new Artist()
        {
            Name = spotifyArtist.Name!,
            Links = linksModel
        };
        return artistModel;
    }
}