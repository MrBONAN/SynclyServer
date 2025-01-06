using System.Text.Json.Serialization;

namespace SpotifyApi.Models
{
    public class PlaybackState
    {
        [JsonPropertyName("item")] public Track? Item { get; set; }

        [JsonPropertyName("currently_playing_type")] public string? CurrentlyPlayingType { get; set; }
    }
}