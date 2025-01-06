using Microsoft.EntityFrameworkCore;
using SynclyServer.Data;
using SynclyServer.Interfaces;
using SynclyServer.Models;

namespace SynclyServer.Repository;

public class LocationRepository(ServerDbContext context) : ILocationRepository
{
    private readonly ServerDbContext Context = context;
    
    public async Task<List<Location>> GetAllAsync()
    {
        return await Context.Locations.ToListAsync();
    }

    public async Task<Location?> GetByIdAsync(int id)
    {
        return await Context.Locations.FindAsync(id);
    }

    public async Task<Location> CreateAsync(Location locationModel)
    {
        await Context.Locations.AddAsync(locationModel);
        await Context.SaveChangesAsync();
        return locationModel;
    }
}