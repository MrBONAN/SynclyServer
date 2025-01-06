using Microsoft.EntityFrameworkCore;
using SynclyServer.Data;
using SynclyServer.Interfaces;
using SynclyServer.Models;

namespace SynclyServer.Repository;

public class TrackRepository(ServerDbContext context, ILinksRepository linksRepository) : ITrackRepository
{
    private readonly ServerDbContext Context = context;
    private readonly ILinksRepository LinksRepository = linksRepository;

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
    
    public async Task<List<Track>> GetByIdsAsync(IEnumerable<string> externalIds)
    {
        return await Context.Tracks
            .Where(t => t.Links != null && externalIds.Contains(t.Links.ExternalId))
            .ToListAsync();
    }

    public async Task<Track> CreateAsync(Track trackModel)
    {
        var linksModel = await LinksRepository.CreateAsync(trackModel.Links!);
        trackModel.LinksId = linksModel.Id;
        trackModel.Links = linksModel;
        await Context.Tracks.AddAsync(trackModel);
        await Context.SaveChangesAsync();
        return trackModel;
    }
}