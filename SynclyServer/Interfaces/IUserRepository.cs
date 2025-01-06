using SynclyServer.Models;
using SynclyServer.Models.Authorization;

namespace SynclyServer.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    public Task<List<Track>?> GetTopTracksAsync(int userId);
    public Task<List<Artist>?> GetTopArtistsAsync(int userId);
    public Task<List<Track>?> GetRecentlyTracksAsync(int userId);
    public Task<User> CreateAsync(User userModel);

    public Task<Registration?> GetRegistrationDataByUserIdAsync(int id);
}