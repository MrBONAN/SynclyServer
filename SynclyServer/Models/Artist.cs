using System.ComponentModel.DataAnnotations.Schema;

namespace SynclyServer.Models;

[Table("Artists")]
public class Artist
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int LinksId { get; set; }
}