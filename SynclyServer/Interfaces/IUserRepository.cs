using SynclyServer.Models;

namespace SynclyServer.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    public Task<List<Track>?> GetTopTracksAsync(int id);
    public Task<List<Artist>?> GetTopArtistsAsync(int id);
}