using RestSharp;
using SpotifyApi.ManageApiResult;
using SpotifyApi.Models;

namespace SpotifyApi;

public static partial class SpotifyApi
{
    public static async Task<SpotifyApiResult<Track, PlaybackState>> GetCurrentTrackAsync(string accessToken)
    {
        var request = new RestRequest("/player");
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        var response = await UserProfileClient.ExecuteGetAsync<PlaybackState>(request);

        if (response.IsSuccessful && response.Data != null)
        {
            if (response.Data.CurrentlyPlayingType == "track")
                return new (ApiResult.Ok, response.Data.Item!, response);
            return new(ApiResult.InvalidCurrentPlayingType, null, response);
        }

        var apiResult = ResultManager.ManageResponseResult(response);
        return new (apiResult, null, response);
    }
}