using Newtonsoft.Json;
using SynclyServer.Dtos.Links;

namespace SynclyServer.Dtos.Artist;

public class ArtistDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    //public List<TrackDto> Tracks { get; set; } = new();
    [JsonIgnore] public int LinksId { get; set; }
    public LinksDto Links { get; set; }
}