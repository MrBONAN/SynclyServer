using Microsoft.EntityFrameworkCore;
using SynclyServer.Data;
using SynclyServer.Interfaces;
using SynclyServer.Models;

namespace SynclyServer.Repository;

public class ArtistRepository(ServerDbContext context) : IArtistRepository
{
    private readonly ServerDbContext Context = context;

    public async Task<List<Artist>> GetAllAsync()
    {
        return await Context.Artists.ToListAsync();
    }

    public async Task<Artist?> GetByIdAsync(int id)
    {
        return await Context.Artists.FindAsync(id);
    }
}