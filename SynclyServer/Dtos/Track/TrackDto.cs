using SynclyServer.Dtos.Artist;

namespace SynclyServer.Dtos.Track;

public class TrackDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ArtistDto> Artists { get; set; } = new();
    public string Genre { get; set; } = string.Empty;
    public int LinksId { get; set; }
}