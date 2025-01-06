using Microsoft.EntityFrameworkCore;
using SpotifyApi.Models.Authorization;
using SynclyServer.Data;
using SynclyServer.Interfaces;
using SynclyServer.Mappers;
using SynclyServer.Models;
using SynclyServer.Models.Authorization;

namespace SynclyServer.Repository;

public class UserRepository(ServerDbContext context, ILocationRepository locationRepository) : IUserRepository
{
    private readonly ServerDbContext Context = context;
    private readonly ILocationRepository LocationRepository = locationRepository;
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

    public async Task<List<Track>?> GetTopTracksAsync(int userId)
    {
        var user = await Context.Users
            .Include(u => u.TopTracks.OrderBy(ua => ua.Position).Take(MaxTopTracks))
            .ThenInclude(ut => ut.Track)
            .ThenInclude(t => t!.Links)
            .FirstOrDefaultAsync(u => u.Id == userId);
        return user?.TopTracks.Select(ut => ut.Track).ToList()!;
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

    public async Task<Registration?> GetRegistrationDataByUserIdAsync(int id)
    {
        var registrationModel = await Context.Users
            .Where(u => u.Id == id)
            .Select(u => new Registration()
            {
                Id = u.RegistrationId,
                AccessToken = u.Registration.AccessToken,
                ExpiresDate = u.Registration.ExpiresDate,
                RefreshToken = u.Registration.RefreshToken
            })
            .FirstOrDefaultAsync();
        if (registrationModel is null)
            return null;
        if ((registrationModel.ExpiresDate - DateTime.Now).TotalSeconds < 10)
        {
            var newAccessToken = await SpotifyApi.SpotifyApi.RefreshTokenAsync(registrationModel.RefreshToken);
            if (newAccessToken is null)
                return null;
            return await UpdateAccessToken(registrationModel, newAccessToken);
        }

        return registrationModel;
    }

    private async Task<Registration> UpdateAccessToken(Registration registrationModel,
        SpotifyAccessTokenDto newAccessToken)
    {
        registrationModel.AccessToken = newAccessToken.AccessToken;
        registrationModel.RefreshToken = newAccessToken.RefreshToken;
        registrationModel.ExpiresDate = newAccessToken.ExpiresDate;
        registrationModel.TokenType = newAccessToken.TokenType;
        await Context.SaveChangesAsync();
        return registrationModel;
    }
}