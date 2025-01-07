using SynclyServer.Models;
using SynclyServer.Models.JoinTables;

namespace SynclyServer.Interfaces;

public interface ITopArtistsRepository
{
    public Task<List<Artist>?> GetTopArtistsAsync(int userId);

    public Task<List<TopArtists>> CreateNewTopArtistsAsync(int userId, IEnumerable<int> artistIds,
        int startIndex);
}