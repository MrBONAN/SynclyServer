using SynclyServer.Models;

namespace SynclyServer.Interfaces;

public interface IArtistRepository
{
    Task<List<Artist>> GetAllAsync();
    Task<Artist?> GetByIdAsync(int id);
    Task<List<Artist>> GetByIdsAsync(IEnumerable<string> externalIds);
    Task<Artist> CreateAsync(Artist artistModel);
}