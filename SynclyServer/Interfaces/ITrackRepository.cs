using SynclyServer.Models;

namespace SynclyServer.Interfaces;

public interface ITrackRepository
{
    Task<List<Track>> GetAllAsync();
    Task<Track?> GetByIdAsync(int id);
}