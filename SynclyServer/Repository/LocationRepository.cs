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
        return await Context.Locations
            .Include(l => l.User)
            .ToListAsync();
    }

    public async Task<Location?> GetByIdAsync(int id)
    {
        return await Context.Locations
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<Location> CreateAsync(Location locationModel)
    {
        await Context.Locations.AddAsync(locationModel);
        await Context.SaveChangesAsync();
        return locationModel;
    }

    public async Task<Location?> UpdateAsync(int userId, Location locationModel)
    {
        var location = await Context.Locations.FirstOrDefaultAsync(l => l.UserId == userId);
        if (location is null)
            return null;
        location.Latitude = location.Latitude;
        location.Longitude = location.Longitude;
        location.LastUpdate = DateTime.UtcNow;
        await Context.SaveChangesAsync();
        return location;
    }
}