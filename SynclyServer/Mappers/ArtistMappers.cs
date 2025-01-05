using SynclyServer.Dtos.Artist;
using SynclyServer.Models;

namespace SynclyServer.Mappers;

public static class ArtistMappers
{
    public static ArtistDto ToArtistDto(this Artist artisModel)
    {
        return new ArtistDto()
        {
            Id = artisModel.Id,
            Name = artisModel.Name,
            //Tracks = artisModel.Tracks.Select(t => t.ToTrackDto()).ToList(),
            LinksId = artisModel.LinksId
        };
    }
}