using System.Text.Json.Serialization;

namespace Sonnet.Clients.PokeApi.Models;

/// <summary>
/// A Pokémon Species forms the basis for at least one Pokémon.
/// Attributes of a Pokémon species are shared across all varieties of Pokémon within the species.
/// </summary>
public class Species
{
    /// <summary>
    /// A list of flavor text entries for this Pokémon species.
    /// </summary>
    [JsonPropertyName("flavor_text_entries")]
    public IEnumerable<FlavorText>? FlavorTextEntries { get; set; }
}