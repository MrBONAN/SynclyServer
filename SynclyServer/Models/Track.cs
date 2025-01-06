using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SynclyServer.Models.JoinTables;

namespace SynclyServer.Models;

[Table("Tracks")]
public class Track
{
    public int Id { get; set; }
    [MaxLength(50)] public string Name { get; set; } = string.Empty;
    public List<TrackArtist>? TrackArtists { get; set; } = new();
    [MaxLength(20)] public string? Genre { get; set; } = string.Empty;
    public int? LinksId { get; set; }
    public Links? Links { get; set; }
}