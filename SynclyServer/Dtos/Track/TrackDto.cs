using Newtonsoft.Json;
using SynclyServer.Dtos.Artist;
using SynclyServer.Dtos.Links;

namespace SynclyServer.Dtos.Track;

public class TrackDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ArtistDto> Artists { get; set; } = new();
    [JsonIgnore] public int LinksId { get; set; }
    public LinksDto Links { get; set; } = null!;
}