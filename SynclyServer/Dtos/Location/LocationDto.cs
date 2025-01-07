using Newtonsoft.Json;

namespace SynclyServer.Dtos.Location;

public class LocationDto
{
    [JsonIgnore] public int Id { get; set; }
    //public int UserId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime LastUpdate { get; set; } = DateTime.Now;
}