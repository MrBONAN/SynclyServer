using System.ComponentModel.DataAnnotations.Schema;

namespace SynclyServer.Models.JoinTables;

[Table("TopTracks")]
public class TopTracks
{
    public int UserId { get; set; }
    public int TrackId { get; set; }
    public User? User { get; set; }
    public Track? Track { get; set; }
    public int Position { get; set; }
}