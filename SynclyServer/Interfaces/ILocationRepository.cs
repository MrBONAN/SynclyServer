using SynclyServer.Models;

namespace SynclyServer.Interfaces;

public interface ILocationRepository
{
    Task<List<Location>> GetAllAsync();
    Task<Location?> GetByIdAsync(int id);
}