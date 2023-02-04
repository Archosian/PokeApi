using System.Text.Json.Serialization;

namespace Sonnet.Clients.PokeApi.Models;

public class FlavorText
{
    [JsonPropertyName("flavor_text")]
    public string? Text { get; set; }
    [JsonPropertyName("language")]
    public string? Lang { get; set; }
    [JsonPropertyName("version")]
    public string? Version { get; set; }
}