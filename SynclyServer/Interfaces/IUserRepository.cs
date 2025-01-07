using SynclyServer.Models;
using SynclyServer.Models.Authorization;

namespace SynclyServer.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    public Task<User> CreateAsync(User userModel);
}