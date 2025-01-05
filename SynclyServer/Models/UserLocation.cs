using System.ComponentModel.DataAnnotations.Schema;

namespace SynclyServer.Models;

[Table("UserLocations")]
public class UserLocation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Latitude { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal Longitude { get; set; }
    public DateTime LastUpdate { get; set; } = DateTime.Now;
}