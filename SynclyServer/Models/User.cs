using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SynclyServer.Models.Authorization;
using SynclyServer.Models.JoinTables;

namespace SynclyServer.Models;

[Table("Users")]
public class User
{
    public int Id { get; set; }
    public int? RegistrationId { get; set; }
    public Registration? Registration { get; set; } = null!;
    [MaxLength(100)] public string Name { get; set; } = string.Empty;
    //public int? CurrentTrackId { get; set; }
    public List<TopTracks> TopTracks { get; set; } = new();
    public List<TopArtists> TopArtists { get; set; } = new();
    public List<RecentlyTracks> RecentlyTracks { get; set; } = new();
    public int? LocationId { get; set; }
    public Location? Location { get; set; }
    public int? LinksId { get; set; }
    public Links? Links { get; set; }

    public DateTime RecentlyTracksUpdateDate { get; set; } = DateTime.MinValue;
    public DateTime TopTracksUpdateDate { get; set; } = DateTime.MinValue;
    public DateTime TopArtistsUpdateDate { get; set; } = DateTime.MinValue;
}