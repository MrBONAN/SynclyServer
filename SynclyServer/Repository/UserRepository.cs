using Microsoft.EntityFrameworkCore;
using SynclyServer.Data;
using SynclyServer.Interfaces;
using SynclyServer.Models;

namespace SynclyServer.Repository;

public class UserRepository(
    ServerDbContext context,
    ILocationRepository locationRepository,
    ITrackRepository trackRepository) : IUserRepository
{
    private readonly ServerDbContext Context = context;
    private readonly ILocationRepository LocationRepository = locationRepository;
    private readonly ITrackRepository TrackRepository = trackRepository;
    private const int MaxTopTracks = 10;
    private const int MaxTopArtists = 10;
    private const int MaxRecentlyTracks = 10;

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

    public async Task<List<Artist>?> GetTopArtistsAsync(int userId)
    {
        var user = await Context.Users
            .Include(u => u.TopArtists.OrderBy(ua => ua.Position).Take(MaxTopArtists))
            .ThenInclude(ua => ua.Artist)
            .ThenInclude(a => a!.Links)
            .FirstOrDefaultAsync(u => u.Id == userId);
        return user?.TopArtists.Select(ua => ua.Artist).ToList()!;
    }

    public async Task<List<Track>?> GetRecentlyTracksAsync(int userId)
    {
        var user = await Context.Users
            .Include(u => u.RecentlyTracks.OrderBy(ut => ut.Position).Take(MaxRecentlyTracks))
            .ThenInclude(ut => ut.Track)
            .ThenInclude(t => t!.Links)
            .FirstOrDefaultAsync(u => u.Id == userId);
        return user?.RecentlyTracks.Select(ut => ut.Track).ToList()!;
    }

    public async Task<User> CreateAsync(User userModel)
    {
        await Context.Users.AddAsync(userModel);
        await Context.SaveChangesAsync();
        userModel.Registration.UserId = userModel.Id;
        await Context.SaveChangesAsync();
        return (await GetByIdAsync(userModel.Id))!;
    }
}