using RestSharp;
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
            return new SpotifyApiResult<List<Track>, PagingRecently>(ApiResult.Success,
                response.Data.TrackMetadata!.Select(tm => tm.Track).ToList(), response);

        return new SpotifyApiResult<List<Track>, PagingRecently>(ApiResult.Error, null, response);
    }
}