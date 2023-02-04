using System.Text.Json.Serialization;

namespace Sonnet.Clients.PokeApi.Models;

/// <summary>
/// The various Sprites for a Pokémon
/// </summary>
public class Sprites
{
    /// <summary>
    /// The default depiction of this Pokémon from the front in battle.
    /// </summary>
    [JsonPropertyName("front_default")]
    public string? Front { get; set; }

    /// <summary>
    /// The default depiction of this Pokémon from the back in battle.
    /// </summary>
    [JsonPropertyName("back_default")]
    public string? Back { get; set; }
}