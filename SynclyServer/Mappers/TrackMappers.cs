using SynclyServer.Dtos.Track;
using SynclyServer.Models;

namespace SynclyServer.Mappers;

public static class TrackMappers
{
    public static TrackDto ToTrackDto(this Track trackModel)
    {
        return new TrackDto()
        {
            Id = trackModel.Id,
            Name = trackModel.Name,
            Artists = trackModel.TrackArtists.Select(ta => ta.Artist.ToArtistDto()).ToList(),
            Genre = trackModel.Genre,
            LinksId = trackModel.LinksId
        };
    }
}