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

    public async Task<Location?> UpdateAsync(int userId, Location locationModel)
    {
        var user = await Context.Users
            .Include(u => u.Location)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            return null;
        user.Location!.Latitude = locationModel.Latitude;
        user.Location!.Longitude = locationModel.Longitude;
        await Context.SaveChangesAsync();
        return user.Location;
    }
}