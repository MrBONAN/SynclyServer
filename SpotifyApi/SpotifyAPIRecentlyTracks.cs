using RestSharp;
using SpotifyApi.ManageApiResult;
using SpotifyApi.Models;

namespace SpotifyApi;

public static partial class SpotifyApi
{
    public static async Task<SpotifyApiResult<List<Track>, PagingRecently>> GetRecentlyTracks(string accessToken,
        int limit = 10)
    {
        var request = new RestRequest("/player/recently-played");
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        request.AddQueryParameter("limit", limit.ToString());

        var response = await UserProfileClient.ExecuteGetAsync<PagingRecently>(request);

        if (response.IsSuccessful && response.Data != null)
            return new(ApiResult.Ok, response.Data.TrackMetadata!.Select(tm => tm.Track).ToList(), response);

        var apiResult = ResultManager.ManageResponseResult(response);
        return new (apiResult, null, response);
    }
}