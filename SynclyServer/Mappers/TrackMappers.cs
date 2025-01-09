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
            Artists = trackModel.TrackArtists?.Select(ta => ta.Artist!.ToArtistDto()).ToList() ?? new(),
            LinksId = trackModel.LinksId!.Value,
            Links = trackModel.Links!.ToLinksDto()
        };
    }
}