using SynclyServer.Models;
using SynclyServer.Models.JoinTables;

namespace SynclyServer.Interfaces;

public interface ITrackArtistRepository
{
    public Task<TrackArtist> CreateNewTrackArtistAsync(TrackArtist trackArtist);
    public Task<List<TrackArtist>> CreateSeveralTrackArtistsAsync(List<(Track, List<Artist>)> trackArtists);
}