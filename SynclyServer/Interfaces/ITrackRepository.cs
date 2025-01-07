using SynclyServer.Models;
using SynclyServer.Models.JoinTables;

namespace SynclyServer.Interfaces;

public interface ITrackRepository
{
    Task<List<Track>> GetAllAsync();
    Task<Track?> GetByIdAsync(int id);
    Task<List<Track>> GetByIdsAsync(IEnumerable<string> externalIds);
    public Task<List<Track>?> GetRecentlyTracksAsync(int userId);

    public Task<List<RecentlyTracks>> CreateNewRecentlyTracksAsync(int userId, IEnumerable<int> trackIds,
        int startIndex);

    Task<Track> CreateAsync(Track trackModel);
}