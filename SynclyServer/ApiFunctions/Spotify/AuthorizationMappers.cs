using SpotifyApi.Models.Authorization;
using SynclyServer.Models.Authorization;

namespace SynclyServer.ApiFunctions.Spotify;

public static class AuthorizationMappers
{
    public static Registration ToRegistration(this SpotifyAccessTokenDto spotifyAccessTokenDto, string spotifyId = "")
    {
        return new Registration()
        {
            ExternalId = spotifyId,
            PlatformName = "spotify",
            AccessToken = spotifyAccessTokenDto.AccessToken,
            TokenType = spotifyAccessTokenDto.TokenType,
            ExpiresDate = spotifyAccessTokenDto.ExpiresDate,
            RefreshToken = spotifyAccessTokenDto.RefreshToken
        };
    }
}