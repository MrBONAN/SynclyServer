namespace SynclyServer.Dtos.Links;

public class LinksDto
{
    public int Id { get; set; }
    public string ExternalImageLink { get; set; } = string.Empty;
    public string ExternalLink { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
    public string PlatformName { get; set; } = string.Empty;
}