using RestSharp;
using SpotifyApi.ManageApiResult;
using SpotifyApi.Models;

namespace SpotifyApi;

public static partial class SpotifyApi
{
    private static RestClient UserProfileClient = new RestClient("https://api.spotify.com/v1/me/");

    public static async Task<SpotifyApiResult<UserProfile>> GetUserProfileAsync(string accessToken)
    {
        var request = new RestRequest();
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        var response = await UserProfileClient.ExecuteGetAsync<UserProfile>(request);

        if (response.IsSuccessful && response.Data != null)
            return new(ApiResult.Ok, response.Data, response);
        var apiResult = ResultManager.ManageResponseResult(response);
        return new(apiResult, null, response);
    }
}