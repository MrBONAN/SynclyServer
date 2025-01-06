using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SynclyServer.Models.Authorization;
using SynclyServer.Models.JoinTables;

namespace SynclyServer.Models;

[Table("Users")]
public class User
{
    public int Id { get; set; }
    public int UserRegistrationId { get; set; }
    public Registration Registration { get; set; } = null!;
    [MaxLength(50)] public string Name { get; set; } = string.Empty;
    public int? CurrentTrackId { get; set; }
    public List<TopTracks> TopTracks { get; set; } = new();
    public List<TopArtists> TopArtists { get; set; } = new();
    public List<RecentlyTracks> RecentlyTracks { get; set; } = new();
    public int? LocationId { get; set; }
    public int? LinksId { get; set; }
    public Links? Links { get; set; }
}