using SynclyServer.Dtos.User;
using SynclyServer.Models;

namespace SynclyServer.Mappers;

public static class UserMappers
{
    public static UserDto ToUserDto(this User userModel)
    {
        return new UserDto
        {
            Id = userModel.Id,
            Name = userModel.Name,
            //CurrentTrackId = userModel.CurrentTrackId,
            LocationId = userModel.LocationId,
            LinksId = userModel.LinksId!.Value,
            Links = userModel.Links!.ToLinksDto(),

            RecentlyTracksUpdateDate = userModel.RecentlyTracksUpdateDate,
            TopTracksUpdateDate = userModel.TopTracksUpdateDate,
            TopArtistsUpdateDate = userModel.TopArtistsUpdateDate
        };
    }
}