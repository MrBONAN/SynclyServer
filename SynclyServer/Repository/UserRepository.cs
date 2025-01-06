using Microsoft.EntityFrameworkCore;
using SynclyServer.Data;
using SynclyServer.Interfaces;
using SynclyServer.Models;

namespace SynclyServer.Repository;

public class UserRepository(ServerDbContext context) : IUserRepository
{
    private readonly ServerDbContext Context = context;
    private const int MaxTopTracks = 10;
    private const int MaxTopArtists = 10;
    

    public async Task<List<User>> GetAllAsync()
    {
        return await Context.Users
            .Include(u => u.Links)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await Context.Users
            .Include(u => u.Links)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<List<Track>?> GetTopTracksAsync(int id)
    {
        var user = await Context.Users
            .Include(u => u.TopTracks.OrderBy(ua => ua.Position).Take(MaxTopArtists))
            .ThenInclude(ut => ut.Track)
            .ThenInclude(t => t!.Links)
            .FirstOrDefaultAsync(u => u.Id == id);
        return user?.TopTracks.Select(ut => ut.Track).ToList()!;
    }
    
    public async Task<List<Artist>?> GetTopArtistsAsync(int id)
    {
        var user = await Context.Users
            .Include(u => u.TopArtists.OrderBy(ua => ua.Position).Take(MaxTopTracks))
            .ThenInclude(ua => ua.Artist)
            .ThenInclude(a => a!.Links)
            .FirstOrDefaultAsync(u => u.Id == id);
        return user?.TopArtists.Select(ua => ua.Artist).ToList()!;
    }
}