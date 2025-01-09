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
    IRegistrationRepository registrationRepository,
    IArtistRepository artistRepository,
    ITrackArtistRepository trackArtistRepository) : ITrackRepository
{
    private readonly ServerDbContext Context = context;
    private readonly ILinksRepository LinksRepository = linksRepository;
    private readonly IRegistrationRepository RegistrationRepository = registrationRepository;
    private readonly IArtistRepository ArtistRepository = artistRepository;
    private readonly ITrackArtistRepository TrackArtistRepository = trackArtistRepository;
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
            .Include(u => u.RecentlyTracks)
            .ThenInclude(rt => rt.Track)
            .ThenInclude(t => t!.TrackArtists)!
            .ThenInclude(ta => ta.Artist)
            .ThenInclude(a => a!.Links)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            return null;
        if ((DateTime.UtcNow - user.RecentlyTracksUpdateDate).TotalHours > 2)
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

        var newTrackIds = recentlyTracks
            .Select(t => t.Id)
            .Where(tId => nonexistentTracks.Contains(tId))
            .ToList();
        if (newTrackIds.Count > 0)
        {
            var addedTracks = await CreateTracksWithArtistsAsync(userModel.Id, newTrackIds!);
            if (addedTracks is null)
                return null;
            tracksInDb.AddRange(addedTracks);
        }

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

        userModel.RecentlyTracksUpdateDate = DateTime.UtcNow;
        await Context.SaveChangesAsync();
        return userModel;
    }

    public async Task<Track> CreateAsync(Track trackModel)
    {
        var existedModel = await FindByExternalId(trackModel.Links!.ExternalId);
        if (existedModel is not null)
            return existedModel;
        var linksModel = await LinksRepository.CreateAsync(trackModel.Links!);
        trackModel.LinksId = linksModel.Id;
        trackModel.Links = linksModel;
        await Context.Tracks.AddAsync(trackModel);
        await Context.SaveChangesAsync();
        return trackModel;
    }

    private async Task<Track?> FindByExternalId(string externalId)
    {
        return await Context.Tracks
            .Include(t => t.Links)
            .FirstOrDefaultAsync(t => t.Links!.ExternalId == externalId);
    }

    public async Task<List<Track>?> CreateTracksWithArtistsAsync(int userId, IEnumerable<string> newTrackIds)
    {
        var tracksInDb = new List<Track>();
        var newTracks = await CreateSeveralTracksBySpotifyIdAsync(userId, newTrackIds!);
        if (newTracks is null)
            return null;
        var newTracksInDb = new List<(Track, List<Artist>)>();
        foreach (var (trackModel, artistModels) in newTracks)
        {
            tracksInDb.Add(await CreateAsync(trackModel));
            var artists = new List<Artist>();
            foreach (var artistModel in artistModels)
                artists.Add(await ArtistRepository.CreateAsync(artistModel));

            newTracksInDb.Add((trackModel, artists));
        }

        await TrackArtistRepository.CreateSeveralTrackArtistsAsync(newTracksInDb);
        return tracksInDb;
    }

    public async Task<(Track, List<Artist>)?> CreateTrackBySpotifyIdAsync(int userId, string spotifyId)
    {
        var registrationData = await RegistrationRepository.GetRegistrationDataByUserIdAsync(userId);
        if (registrationData is null)
            return null;
        var fullTrackInfoResult =
            await SpotifyApi.SpotifyApi.GetTrackByIdWithFullArtistsInfoAsync(registrationData.AccessToken, spotifyId);
        if (fullTrackInfoResult.Result is not ApiResult.Ok)
            return null;
        return ModelsCreator.CreateTrackWithArtists(fullTrackInfoResult.Data!);
    }

    public async Task<List<(Track, List<Artist>)>?> CreateSeveralTracksBySpotifyIdAsync(int userId,
        IEnumerable<string> spotifyIds)
    {
        var registrationData = await RegistrationRepository.GetRegistrationDataByUserIdAsync(userId);
        if (registrationData is null)
            return null;
        var fullTrackInfoResult =
            await SpotifyApi.SpotifyApi.GetSeveralTrackByIdsWithFullArtistsInfoAsync(registrationData.AccessToken,
                spotifyIds);
        if (fullTrackInfoResult.Result is not ApiResult.Ok)
            return null;
        return ModelsCreator.CreateSeveralTracksWithArtists(fullTrackInfoResult.Data!);
    }
}