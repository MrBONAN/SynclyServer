using SynclyServer.Models;

namespace SynclyServer.Interfaces;

public interface ITrackRepository
{
    Task<List<Track>> GetAllAsync();
    Task<Track?> GetByIdAsync(int id);
    Task<List<Track>> GetByIdsAsync(IEnumerable<string> externalIds);
    Task<Track> CreateAsync(Track trackModel);
}