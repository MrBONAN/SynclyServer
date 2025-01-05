using SynclyServer.Dtos.Links;
using SynclyServer.Models;

namespace SynclyServer.Mappers;

public static class LinksMappers
{
    public static LinksDto ToLinksDto(this Links linksModel)
    {
        return new LinksDto
        {
            Id = linksModel.Id,
            ExternalImageLink = linksModel.ExternalImageLink,
            ExternalLink = linksModel.ExternalLink,
            ExternalId = linksModel.ExternalId,
            PlatformName = linksModel.PlatformName
        };
    }
}