using RestSharp;
using SpotifyApi.ManageApiResult;
using SpotifyApi.Models;

namespace SpotifyApi;

public static partial class SpotifyApi
{
    public static async Task<SpotifyApiResult<T>> GetEntityByIdAsync<T>(string accessToken, string id) where T : class
    {
        var request = new RestRequest($"/{id}");
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        var response = await UserProfileClient.ExecuteGetAsync<T>(request);

        if (response.IsSuccessful && response.Data != null)
            return new(ApiResult.Ok, response.Data, response);

        var apiResult = ResultManager.ManageResponseResult(response);
        return new(apiResult, null, response);
    }

    public static async Task<SpotifyApiResult<List<T>, TracksAndArtistsList<T>>> GetSeveralEntitiesById<T>(
        string accessToken,
        IEnumerable<string> ids) where T : class
    {
        var type = typeof(T) == typeof(Track) ? "tracks" : "artists";
        var request = new RestRequest($"{type}?ids={string.Join(',', ids)}");
        request.AddHeader("Authorization", $"Bearer {accessToken}");

        var response = await SpotifyApi.SpotifyClient.ExecuteGetAsync<TracksAndArtistsList<T>>(request);

        if (response.IsSuccessful && response.Data != null)
            return new(ApiResult.Ok, response.Data.Tracks ?? response.Data.Artists, response);

        var apiResult = ResultManager.ManageResponseResult(response);
        return new(apiResult, null, response);
    }

    public static async Task<SpotifyApiResult<Track>> GetTrackByIdWithFullArtistsInfoAsync(string accessToken,
        string trackId)
    {
        var trackResponse = await GetEntityByIdAsync<Track>(accessToken, trackId);
        if (trackResponse.Result is not ApiResult.Ok)
            return new(trackResponse.Result, null, null);
        var trackData = trackResponse.Data!;
        var artistsIds = trackData.Artists!.Select(a => a.Id);
        var artistsResponse = await GetSeveralEntitiesById<Artist>(accessToken, artistsIds!);
        if (artistsResponse.Result is not ApiResult.Ok)
            return new(artistsResponse.Result, null, null);
        trackData.Artists = artistsResponse.Data;
        return new(ApiResult.Ok, trackData, null);
    }

    public static async Task<SpotifyApiResult<List<Track>>> GetSeveralTrackByIdsWithFullArtistsInfoAsync(
        string accessToken,
        IEnumerable<string> trackIds)
    {
        var tracksResponse = await GetSeveralEntitiesById<Track>(accessToken, trackIds);
        if (tracksResponse.Result is not ApiResult.Ok)
            return new(tracksResponse.Result, null, null);
        var tracksData = tracksResponse.Data!;
        var artistsMapper = tracksData
            .SelectMany((t, i) => t.Artists!.Select(a => (artistId: a.Id!, trackIndex: i)))
            .ToLookup(tuple => tuple.artistId);
        var artistsIds = tracksData.SelectMany(t => t.Artists!.Select(a => a.Id));
        var artistsResponse = await GetSeveralEntitiesById<Artist>(accessToken, artistsIds!);
        if (artistsResponse.Result is not ApiResult.Ok)
            return new(artistsResponse.Result, null, null);
        foreach (var track in tracksData)
            track.Artists!.Clear();
        var alreadyWas = new HashSet<string>();
        foreach (var artistModel in artistsResponse.Data!)
        {
            if (alreadyWas.Contains(artistModel.Id!)) continue;
            alreadyWas.Add(artistModel.Id!);
            foreach(var tuple in artistsMapper[artistModel.Id!])
                tracksData[tuple.trackIndex].Artists!.Add(artistModel);
        }

        return new(ApiResult.Ok, tracksData, null);
    }
}