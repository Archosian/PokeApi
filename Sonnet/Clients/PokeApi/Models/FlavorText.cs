using System.Text.Json.Serialization;

namespace Sonnet.Clients.PokeApi.Models;

public class FlavorText
{
    /// <summary>
    /// The localized flavor text for an API resource in a specific language.
    /// Note that this text is left unprocessed as it is found in game files.
    /// This means that it contains special characters that one might want to replace with their visible decodable version.
    /// </summary>
    [JsonPropertyName("flavor_text")]
    public string? Text { get; set; }

    /// <summary>
    /// The game version this flavor text is extracted from.
    /// </summary>
    [JsonPropertyName("version")]
    public NamedApiResource? Version { get; set; }
}