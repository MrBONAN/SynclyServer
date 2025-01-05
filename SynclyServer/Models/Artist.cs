using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SynclyServer.Models;

[Table("Artists")]
public class Artist
{
    public int Id { get; set; }
    [MaxLength(50)] public string Name { get; set; } = string.Empty;
    public List<Track> Tracks { get; set; } = new();
    public int LinksId { get; set; }
}