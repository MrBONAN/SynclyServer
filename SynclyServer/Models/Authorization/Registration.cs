using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SynclyServer.Models.Authorization;

[Table("UserRegistrations")]
public class Registration
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; } = null!;
    [MaxLength(50)] public string ExternalId { get; set; } = null!;
    [MaxLength(10)] public string PlatformName { get; set; } = null!;

    [MaxLength(300)] public string AccessToken { get; set; } = null!;
    [MaxLength(10)] public string TokenType { get; set; } = null!;
    public DateTime ExpiresDate { get; set; }
    [MaxLength(200)] public string RefreshToken { get; set; } = null!;
}