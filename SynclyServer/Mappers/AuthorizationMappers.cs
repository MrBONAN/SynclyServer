using SynclyServer.Dtos.Authorization.Spotify;
using SynclyServer.Models.Authorization;

namespace SynclyServer.Mappers;

public static class AuthorizationMappers
{
    public static Registration ToRegistration(this SpotifyAccessTokenDto spotifyAccessTokenDto)
    {
        return new Registration()
        {
            AccessToken = spotifyAccessTokenDto.AccessToken,
            TokenType = spotifyAccessTokenDto.TokenType,
            ExpiresDate = spotifyAccessTokenDto.ExpiresDate,
            RefreshToken = spotifyAccessTokenDto.RefreshToken
        };
    }
}