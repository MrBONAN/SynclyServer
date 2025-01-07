using Microsoft.EntityFrameworkCore;
using SpotifyApi;
using SynclyServer.Data;
using SynclyServer.Interfaces;
using SynclyServer.Mappers;
using SynclyServer.Models;
using SynclyServer.Models.JoinTables;

namespace SynclyServer.Repository;

public class TopArtistsRepository(
    ServerDbContext context,
    IRegistrationRepository registrationRepository,
    IArtistRepository artistRepository) : ITopArtistsRepository
{
    private readonly ServerDbContext Context = context;
    private readonly IRegistrationRepository RegistrationRepository = registrationRepository;
    private readonly IArtistRepository ArtistRepository = artistRepository;
    private const int MaxTopArtists = 10;

    public async Task<List<Artist>?> GetTopArtistsAsync(int userId)
    {
        var user = await Context.Users
            .Include(u => u.TopArtists.OrderBy(ua => ua.Position).Take(MaxTopArtists))
            .ThenInclude(ua => ua.Artist)
            .ThenInclude(a => a!.Links)
            .Include(u => u.Registration)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            return null;
        if ((DateTime.Now - user.TopItemsUpdateDate).TotalDays < 1)
            return user.TopArtists.Select(ua => ua.Artist).ToList()!;
        user = await UpdateUserTopArtists(user);
        if (user is null)
            return null;

        return user.TopArtists.Select(ua => ua.Artist).ToList()!;
    }

    public async Task<List<TopArtists>> CreateNewTopArtistsAsync(int userId, IEnumerable<int> artistIds, int startIndex)
    {
        var topArtists = new List<TopArtists>();
        foreach (var artistId in artistIds)
        {
            var topArtist = new TopArtists()
            {
                UserId = userId,
                ArtistId = artistId,
                Position = startIndex++
            };
            Context.TopArtists.Add(topArtist);
            topArtists.Add(topArtist);
        }

        await Context.SaveChangesAsync();
        return topArtists;
    }

    private async Task<User?> UpdateUserTopArtists(User userModel)
    {
        var registrationData = await RegistrationRepository.GetRegistrationDataByUserIdAsync(userModel.Id);

        var topArtists =
            await SpotifyApi.SpotifyApi.GetUserTopItemsAsync<SpotifyApi.Models.Artist>(registrationData!.AccessToken,
                MaxTopArtists);
        if (topArtists.Result is not ApiResult.Success)
            return null;
        var topList = topArtists.Data!;

        var artistsInDb = await ArtistRepository.GetByIdsAsync(topList.Select(a => a.Id)!);
        var nonexistentArtists = topList.Select(a => a.Id).Except(artistsInDb.Select(a => a.Links!.ExternalId));
        var newArtists = topList
            .Where(a => nonexistentArtists.Contains(a.Id))
            .Select(ModelsCreator.CreateArtist);
        foreach (var artistModel in newArtists)
            artistsInDb.Add(await ArtistRepository.CreateAsync(artistModel));
        var artistsInDbByPositionsInTop = artistsInDb
            .Select(artist => (topList.FindIndex(tl => tl.Id == artist.Links!.ExternalId), artist: artist))
            .OrderBy(ia => ia.Item1)
            .ToList();

        // Заменяем индексы в текущем топе артистов
        foreach (var (topArtist, (_, artist)) in userModel.TopArtists.Zip(artistsInDbByPositionsInTop))
            topArtist.ArtistId = artist.Id;
        // Создаём новых авторов в топе
        if (artistsInDb.Count > userModel.TopArtists.Count)
        {
            var startIndex = userModel.TopArtists.Count + 1;
            await CreateNewTopArtistsAsync(userModel.Id,
                artistsInDbByPositionsInTop.Select(ia => ia.artist.Id),
                startIndex);
        }

        await Context.SaveChangesAsync();
        return userModel;
    }
}