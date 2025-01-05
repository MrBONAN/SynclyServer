using System.ComponentModel.DataAnnotations.Schema;

namespace SynclyServer.Models;

[Table("Tracks")]
public class Track
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ArtistId { get; set; }
    public string Genre { get; set; } = string.Empty;
    public int LinksId { get; set; }
}