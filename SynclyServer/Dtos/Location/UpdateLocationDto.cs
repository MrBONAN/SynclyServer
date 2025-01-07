using Newtonsoft.Json;

namespace SynclyServer.Dtos.Location;

public class UpdateLocationDto
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}