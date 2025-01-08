using System.ComponentModel.DataAnnotations.Schema;

namespace SynclyServer.Models.JoinTables;

[Table("TopArtists")]
public class TopArtists
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ArtistId { get; set; }
    public User? User { get; set; }
    public Artist? Artist { get; set; }
    public int Position { get; set; }
}