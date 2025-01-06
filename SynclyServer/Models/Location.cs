using System.ComponentModel.DataAnnotations.Schema;

namespace SynclyServer.Models;

[Table("Locations")]
public class Location
{
    public int Id { get; set; }
    public int UserId { get; set; }
    [Column(TypeName = "decimal(9,6)")] public decimal? Latitude { get; set; }
    [Column(TypeName = "decimal(9,6)")] public decimal? Longitude { get; set; }
    public DateTime LastUpdate { get; set; } = DateTime.Now;
}