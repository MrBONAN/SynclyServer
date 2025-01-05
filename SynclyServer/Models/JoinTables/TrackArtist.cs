using System.ComponentModel.DataAnnotations.Schema;

namespace SynclyServer.Models.JoinTables;

[Table("JoinTrackArtist")]
public class TrackArtist
{
    public int TrackId { get; set; }
    public int ArtistId { get; set; }
    public Track Track { get; set; } = null!;
    public Artist Artist { get; set; } = null!;
}