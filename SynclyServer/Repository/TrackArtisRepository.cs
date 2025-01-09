using SynclyServer.Data;
using SynclyServer.Interfaces;
using SynclyServer.Mappers;
using SynclyServer.Models;
using SynclyServer.Models.JoinTables;

namespace SynclyServer.Repository;

public class TrackArtisRepository(ServerDbContext context) : ITrackArtistRepository
{
    private readonly ServerDbContext Context = context;

    public async Task<TrackArtist> CreateNewTrackArtistAsync(TrackArtist trackArtist)
    {
        trackArtist.Track = null;
        trackArtist.Artist = null;
        Console.WriteLine(trackArtist.ArtistId);
        Console.WriteLine(trackArtist.TrackId);
        await Context.TrackArtists.AddAsync(trackArtist);
        await Context.SaveChangesAsync();
        return trackArtist;
    }

    public async Task<List<TrackArtist>> CreateSeveralTrackArtistsAsync(List<(Track, List<Artist>)> trackArtists)
    {
        var trackArtist = new List<TrackArtist>();
        foreach (var (track, artists) in trackArtists)
        foreach (var artist in artists)
            trackArtist.Add(await CreateNewTrackArtistAsync(ModelsCreator.CreateTrackArtist(track, artist)));
        return trackArtist;
    }
}