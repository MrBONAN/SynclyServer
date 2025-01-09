using App.UserAuthorization.SpotifyAuthorization.Models;
using RestSharp;
using SpotifyApi.Models.Authorization;

namespace SpotifyApi;

public static partial class SpotifyApi
{
    public static async Task<SpotifyAccessTokenDto?> RefreshTokenAsync(string refreshToken)
    {
        var client = new RestClient("https://accounts.spotify.com/api/token");
    
        var request = new RestRequest()
            .AddHeader("Content-Type", "application/x-www-form-urlencoded")
            .AddParameter("grant_type", "refresh_token")
            .AddParameter("refresh_token", refreshToken)
            .AddParameter("client_id", ClientId);
    
        var response = await client.ExecutePostAsync<PkceAccessToken>(request);
    
        if (response.IsSuccessful && response.Data != null)
        {
            Console.WriteLine("Токен успешно обновлён");
            return response.Data.ConvertToAccessTokenDto();
        }

        if (response.Content.Contains("Refresh token revoked"))
            return new SpotifyAccessTokenDto() {RefreshToken = refreshToken};
    
        Console.WriteLine($"Ошибка при обновлении токена: {response.ErrorMessage ?? response.Content}");
        return null;
    }
}