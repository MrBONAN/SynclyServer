using SynclyServer.Dtos.Track;

namespace SynclyServer.Dtos.Artist;

public class ArtistDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<TrackDto> Tracks { get; set; } = new();
    public int LinksId { get; set; }
}