using System.Text.Json.Serialization;

namespace SpotifyApi.Models;

public class PagingRecently
{
    [JsonPropertyName("items")] public List<TrackMetadata>? TrackMetadata { get; set; }
}

public class TrackMetadata
{
    [JsonPropertyName("track")] public Track Track { get; set; }
}