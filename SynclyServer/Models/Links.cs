using System.ComponentModel.DataAnnotations.Schema;

namespace SynclyServer.Models;

[Table("ServiceLinks")]
public class Links
{
    public int Id { get; set; }
    public string SpotifyImageLink { get; set; } = string.Empty;
    public string SpotifyLink { get; set; } = string.Empty;
}