using Microsoft.EntityFrameworkCore;
using SpotifyApi;
using SynclyServer.Data;
using SynclyServer.Interfaces;
using SynclyServer.Mappers;
using SynclyServer.Models;
using SynclyServer.Models.JoinTables;

namespace SynclyServer.Repository;

public class TopTrackRepository(
    ServerDbContext context,
    IRegistrationRepository registrationRepository,
    ITrackRepository trackRepository,
    IArtistRepository artistRepository,
    ITrackArtistRepository trackArtistRepository) : ITopTracksRepository
{
    private readonly ServerDbContext Context = context;
    private readonly IRegistrationRepository RegistrationRepository = registrationRepository;
    private readonly ITrackRepository TrackRepository = trackRepository;
    private readonly IArtistRepository ArtistRepository = artistRepository;
    private readonly ITrackArtistRepository TrackArtistRepository = trackArtistRepository;
    private const int MaxTopTracks = 10;

    public async Task<List<Track>?> GetTopTracksAsync(int userId)
    {
        var user = await Context.Users
            .Include(u => u.TopTracks.OrderBy(ua => ua.Position).Take(MaxTopTracks))
            .ThenInclude(ut => ut.Track)
            .ThenInclude(t => t!.Links)
            .Include(u => u.Registration)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            return null;
        if ((DateTime.UtcNow - user.TopTracksUpdateDate).TotalDays > 1)
        {
            user = await UpdateUserTopTracks(user);
            if (user is null)
                return null;
        }

        return user.TopTracks.Select(ut => ut.Track).ToList()!;
    }

    public async Task<List<TopTracks>> CreateNewTopTracksAsync(int userId, IEnumerable<int> trackIds,
        int startIndex)
    {
        var topTracks = new List<TopTracks>();
        foreach (var trackId in trackIds)
        {
            var topTrack = new TopTracks()
            {
                UserId = userId,
                TrackId = trackId,
                Position = startIndex++
            };
            await Context.TopTracks.AddAsync(topTrack);
            topTracks.Add(topTrack);
        }

        await Context.SaveChangesAsync();
        return topTracks;
    }

    private async Task<User?> UpdateUserTopTracks(User userModel)
    {
        var registrationData = await RegistrationRepository.GetRegistrationDataByUserIdAsync(userModel.Id);
        if (registrationData is null)
            return null;
        var topTracks =
            await SpotifyApi.SpotifyApi.GetUserTopItemsAsync<SpotifyApi.Models.Track>(registrationData!.AccessToken,
                MaxTopTracks);
        if (topTracks.Result is not ApiResult.Ok)
            return null;
        var topList = topTracks.Data!;

        var tracksInDb = await TrackRepository.GetByIdsAsync(topList.Select(t => t.Id)!);
        var nonexistentTracks = topList
            .Select(t => t.Id)
            .Except(tracksInDb.Select(t => t.Links!.ExternalId));
        
        var newTrackIds = topList
            .Select(t => t.Id)
            .Where(tId => nonexistentTracks.Contains(tId))
            .ToList();
        if (newTrackIds.Count > 0)
        {
            var addedTracks = await TrackRepository.CreateTracksWithArtistsAsync(userModel.Id, newTrackIds!);
            if (addedTracks is null)
                return null;
            tracksInDb.AddRange(addedTracks);
        }

        var tracksInDbByPositionsInTop = tracksInDb
            .Select(track => (topList.FindIndex(tl => tl.Id == track.Links!.ExternalId), track))
            .OrderBy(it => it.Item1)
            .ToList();

        // Заменяем индексы в текущем топе треков
        foreach (var (topTrack, (_, track)) in userModel.TopTracks.Zip(tracksInDbByPositionsInTop))
        {
            Context.Entry(topTrack).State = EntityState.Detached;
            topTrack.TrackId = track.Id;
        }

        // Создаём новые треки в топе
        if (tracksInDb.Count > userModel.TopTracks.Count)
        {
            var startIndex = userModel.TopTracks.Count + 1;
            await CreateNewTopTracksAsync(userModel.Id,
                tracksInDbByPositionsInTop.Select(it => it.track.Id),
                startIndex);
        }

        userModel.TopTracksUpdateDate = DateTime.Today;
        await Context.SaveChangesAsync();
        return userModel;
    }
}