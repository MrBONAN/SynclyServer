using Microsoft.EntityFrameworkCore;
using SynclyServer.Data;
using SynclyServer.Interfaces;
using SynclyServer.Models;

namespace SynclyServer.Repository;

public class LinksRepository(ServerDbContext context) : ILinksRepository
{
    private readonly ServerDbContext Context = context;

    public async Task<List<Links>> GetAllAsync()
    {
        return await Context.Links.ToListAsync();
    }

    public async Task<Links?> GetByIdAsync(int id)
    {
        return await Context.Links.FindAsync(id);
    }

    public async Task<Links> CreateAsync(Links links)
    {
        await Context.Links.AddAsync(links);
        await Context.SaveChangesAsync();
        return links;
    }
}