using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.API.SpotifyAPI.Models;
using SynclyServer.Dtos.Authorization.Spotify;

namespace SynclyServer.Models.Authorization;

[Table("UserRegistrations")]
public class Registration
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; } = null!;
    public string ExternalId { get; set; } = null!;
    public string PlatformName { get; set; } = null!;

    public string AccessToken { get; set; } = null!;
    public string TokenType { get; set; } = null!;
    public DateTime ExpiresDate { get; set; }
    public string RefreshToken { get; set; } = null!;
}