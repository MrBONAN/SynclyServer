using SynclyServer.Models;

namespace SynclyServer.Interfaces;

public interface ILinksRepository
{
    Task<List<Links>> GetAllAsync();
    Task<Links?> GetByIdAsync(int id);
    Task<Links> CreateAsync(Links links);
}