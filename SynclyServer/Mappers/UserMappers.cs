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
            LocationId = userModel.LocationId,
            LinksId = userModel.LinksId,
            Links = userModel.Links.ToLinksDto()
        };
    }
}