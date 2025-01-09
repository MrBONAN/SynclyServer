using Microsoft.EntityFrameworkCore;
using SpotifyApi;
using SynclyServer.Data;
using SynclyServer.Interfaces;
using SynclyServer.Mappers;
using SynclyServer.Models;
using SynclyServer.Models.JoinTables;

namespace SynclyServer.Repository;

public class TrackRepository(
    ServerDbContext context,
    ILinksRepository linksRepository,
    IRegistrationRepository registrationRepository) : ITrackRepository
{
    private readonly ServerDbContext Context = context;
    private readonly ILinksRepository LinksRepository = linksRepository;
    private readonly IRegistrationRepository RegistrationRepository = registrationRepository;
    private const int MaxRecentlyTracks = 10;

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
            .Include(t => t.Links)
            .Where(t => t.Links != null && externalIds.Contains(t.Links.ExternalId))
            .ToListAsync();
    }

    public async Task<List<Track>?> GetRecentlyTracksAsync(int userId)
    {
        var user = await Context.Users
            .Include(u => u.RecentlyTracks.OrderBy(ut => ut.Position).Take(MaxRecentlyTracks))
            .ThenInclude(ut => ut.Track)
            .ThenInclude(t => t!.Links)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            return null;
        if ((DateTime.Now - user.RecentlyTracksUpdateDate).TotalHours > 2)
        {
            user = await UpdateUserRecentlyTracks(user);
            if (user is null)
                return null;
        }

        return user.RecentlyTracks.Select(ut => ut.Track).ToList()!;
    }

    public async Task<List<RecentlyTracks>> CreateNewRecentlyTracksAsync(int userId, IEnumerable<int> trackIds,
        int startIndex)
    {
        var recTracks = new List<RecentlyTracks>();
        foreach (var trackId in trackIds)
        {
            var topTrack = new RecentlyTracks()
            {
                UserId = userId,
                TrackId = trackId,
                Position = startIndex++
            };
            Context.RecentlyTracks.Add(topTrack);
            recTracks.Add(topTrack);
        }

        await Context.SaveChangesAsync();
        return recTracks;
    }

    private async Task<User?> UpdateUserRecentlyTracks(User userModel)
    {
        var registrationData = await RegistrationRepository.GetRegistrationDataByUserIdAsync(userModel.Id);
        if (registrationData is null)
            return null;
        var response =
            await SpotifyApi.SpotifyApi.GetRecentlyTracks(registrationData!.AccessToken, MaxRecentlyTracks);
        if (response.Result is not ApiResult.Ok)
            return null;
        var recentlyTracks = response.Data!;

        var tracksInDb = await GetByIdsAsync(recentlyTracks.Select(t => t.Id)!);
        var nonexistentTracks = recentlyTracks
            .Select(t => t.Id)
            .Except(tracksInDb.Select(t => t.Links!.ExternalId))
            .ToList();
        var newTracks = recentlyTracks
            .Where(t => nonexistentTracks.Contains(t.Id))
            .Select(ModelsCreator.CreateTrack)
            .ToList();
        foreach (var trackModel in newTracks)
            tracksInDb.Add(await CreateAsync(trackModel));
        var tracksInDbByPositionsInList = tracksInDb
            .Select(track => (recentlyTracks.FindIndex(tl => tl.Id == track.Links!.ExternalId), track))
            .OrderBy(it => it.Item1)
            .ToList();
        // Заменяем индексы в текущем списке треков
        foreach (var (recTrack, (_, track)) in userModel.RecentlyTracks.Zip(tracksInDbByPositionsInList))
        {
            Context.Entry(recTrack).State = EntityState.Detached;
            recTrack.TrackId = track.Id;
        }
        // Создаём новые треки в топе
        if (tracksInDb.Count > userModel.RecentlyTracks.Count)
        {
            var startIndex = userModel.RecentlyTracks.Count + 1;
            await CreateNewRecentlyTracksAsync(userModel.Id,
                tracksInDbByPositionsInList.Select(it => it.track.Id),
                startIndex);
        }

        userModel.RecentlyTracksUpdateDate = DateTime.Now;
        await Context.SaveChangesAsync();
        return userModel;
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