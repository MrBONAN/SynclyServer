using Microsoft.EntityFrameworkCore;
using SynclyServer.Data;
using SynclyServer.Interfaces;
using SynclyServer.Models;

namespace SynclyServer.Repository;

public class ArtistRepository(ServerDbContext context, ILinksRepository linksRepository) : IArtistRepository
{
    private readonly ServerDbContext Context = context;
    private readonly ILinksRepository LinksRepository = linksRepository;

    public async Task<List<Artist>> GetAllAsync()
    {
        return await Context.Artists
            .Include(a => a.Links)
            .ToListAsync();
    }

    public async Task<Artist?> GetByIdAsync(int id)
    {
        return await Context.Artists
            .Include(a => a.Links)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Artist>> GetByIdsAsync(IEnumerable<string> externalIds)
    {
        return await Context.Artists
            .Include(a => a.Links)
            .Where(a => a.Links != null && externalIds.Contains(a.Links.ExternalId))
            .ToListAsync();
    }

    public async Task<Artist> CreateAsync(Artist artistModel)
    {
        var existedModel = await FindByExternalId(artistModel.Links!.ExternalId);
        if (existedModel is not null)
            return existedModel;
        var linksModel = await LinksRepository.CreateAsync(artistModel.Links!);
        artistModel.LinksId = linksModel.Id;
        artistModel.Links = linksModel;
        await Context.Artists.AddAsync(artistModel);
        await Context.SaveChangesAsync();
        return artistModel;
    }

    private async Task<Artist?> FindByExternalId(string externalId)
    {
        return await Context.Artists
            .Include(a => a.Links)
            .FirstOrDefaultAsync(a => a.Links!.ExternalId == externalId);
    }
}