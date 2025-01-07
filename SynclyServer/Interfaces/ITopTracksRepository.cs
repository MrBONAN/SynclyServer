using SynclyServer.Models;
using SynclyServer.Models.JoinTables;

namespace SynclyServer.Interfaces;

public interface ITopTracksRepository
{
    public Task<List<Track>?> GetTopTracksAsync(int userId);

    public Task<List<TopTracks>> CreateNewTopTracksAsync(int userId, IEnumerable<int> trackIds,
        int startIndex);
}