using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SynclyServer.Models;

[Table("Users")]
public class User
{
    public int Id { get; set; }
    [MaxLength(50)] public string Name { get; set; } = string.Empty;
    public int LinksId { get; set; }
    public Links Links { get; set; }
}