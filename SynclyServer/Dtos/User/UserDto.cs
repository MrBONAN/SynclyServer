using SynclyServer.Dtos.Links;

namespace SynclyServer.Dtos.User;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int LinksId { get; set; }
    public LinksDto Links { get; set; }
}