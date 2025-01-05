using Microsoft.EntityFrameworkCore;
using SynclyServer.Data;
using SynclyServer.Interfaces;
using SynclyServer.Models;

namespace SynclyServer.Repository;

public class TrackRepository(ServerDbContext context) : ITrackRepository
{
    private readonly ServerDbContext Context = context;

    public async Task<List<Track>> GetAllAsync()
    {
        return await Context.Tracks
            .Include(t => t.TrackArtists)
            .ThenInclude(ta => ta.Artist)
            .ThenInclude(a => a.Links)
            .Include(t => t.Links)
            .ToListAsync();
    }

    public async Task<Track?> GetByIdAsync(int id)
    {
        return await Context.Tracks
            .Include(t => t.TrackArtists)
            .ThenInclude(ta => ta.Artist)
            .ThenInclude(a => a.Links)
            .Include(t => t.Links)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}