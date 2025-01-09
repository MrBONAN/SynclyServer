using App.UserAuthorization.SpotifyAuthorization.Models;
using RestSharp;
using SpotifyApi.ManageApiResult;
using SpotifyApi.Models.Authorization;

namespace SpotifyApi;

public static partial class SpotifyApi
{
    public static async Task<SpotifyApiResult<SpotifyAccessTokenDto?, PkceAccessToken>> RefreshTokenAsync(
        string refreshToken)
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
            var accessTokenDto = response.Data.ConvertToAccessTokenDto();
            return new(ApiResult.Ok, accessTokenDto, response);
        }
        
        if (response.Content is null)
        {
            var apiResutl = ResultManager.ManageResponseResult(response);
            return new(apiResutl, null, response);
        }

        if (response.Content.Contains("Refresh token revoked"))
            return new(ApiResult.RefreshTokenStillValid, null, response);

        Console.WriteLine($"Ошибка при обновлении токена: {response.ErrorMessage ?? response.Content}");
        return new(ApiResult.RefreshTokenError, null, response);;
    }
}