using System.Text.Json.Serialization;

namespace Sonnet.Models;

public class Sprites
{
    [JsonPropertyName("front_default")]
    public string? Front { get; set; }
    [JsonPropertyName("back_default")]
    public string? Back { get; set; }
}