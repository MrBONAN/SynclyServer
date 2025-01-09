using SynclyServer.Models;
using SynclyServer.Models.JoinTables;

namespace SynclyServer.Interfaces;

public interface ITrackRepository
{
    Task<List<Track>> GetAllAsync();
    Task<Track?> GetByIdAsync(int id);
    Task<List<Track>> GetByIdsAsync(IEnumerable<string> externalIds);
    public Task<List<Track>?> GetRecentlyTracksAsync(int userId);
    Task<(Track, List<Artist>)?> CreateTrackBySpotifyIdAsync(int userId, string spotifyId);
    Task<List<(Track, List<Artist>)>?> CreateSeveralTracksBySpotifyIdAsync(int userId, IEnumerable<string> spotifyIds);

    public Task<List<RecentlyTracks>> CreateNewRecentlyTracksAsync(int userId, IEnumerable<int> trackIds,
        int startIndex);

    Task<Track> CreateAsync(Track trackModel);
    Task<List<Track>?> CreateTracksWithArtistsAsync(int userId, IEnumerable<string> newTrackIds);
}