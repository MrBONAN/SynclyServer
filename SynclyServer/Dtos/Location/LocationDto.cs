using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SynclyServer.Dtos.Location;

public class LocationDto
{
    [JsonIgnore] public int Id { get; set; }
    public int UserId { get; set; }
    [Required]
    [Range(-91, 91)]
    public decimal Latitude { get; set; }
    [Required]
    [Range(-181, 181)]
    public decimal Longitude { get; set; }
    public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
}