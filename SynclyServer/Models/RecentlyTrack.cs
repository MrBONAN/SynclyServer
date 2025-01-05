using System.ComponentModel.DataAnnotations.Schema;

namespace SynclyServer.Models;

[Table("RecentlyTracks")]
public class RecentlyTrack
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TrackId { get; set; }
    public DateTime UpdateTime { get; set; } = DateTime.Now;
}