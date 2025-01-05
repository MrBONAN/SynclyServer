using SynclyServer.Dtos.Links;
using SynclyServer.Models;

namespace SynclyServer.Mappers;

public static class LinksMappers
{
    public static LinksDto ToLinksDto(this Links links)
    {
        return new LinksDto
        {
            Id = links.Id,
            SpotifyImageLink = links.SpotifyImageLink,
            SpotifyLink = links.SpotifyLink
        };
    }
}