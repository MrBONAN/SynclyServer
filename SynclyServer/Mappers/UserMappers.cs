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
            // TODO заменить на возврат из таблицы Links
            LinksId = userModel.LinksId,
            Links = userModel.Links.ToLinksDto()
        };
    }
}