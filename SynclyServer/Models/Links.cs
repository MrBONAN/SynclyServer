using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SynclyServer.Models;

[Table("ServiceLinks")]
public class Links
{
    public int Id { get; set; }
    [MaxLength(100)] public string ExternalImageLink { get; set; } = string.Empty;
    [MaxLength(100)] public string ExternalLink { get; set; } = string.Empty;
    [MaxLength(50)] public string ExternalId { get; set; } = string.Empty;
    [MaxLength(10)] public string PlatformName { get; set; } = string.Empty;
}