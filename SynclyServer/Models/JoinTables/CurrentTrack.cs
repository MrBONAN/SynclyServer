using System.ComponentModel.DataAnnotations.Schema;

namespace SynclyServer.Models.JoinTables;

[Table("CurrentTracks")]
public class CurrentTrack
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TrackId { get; set; }
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
}