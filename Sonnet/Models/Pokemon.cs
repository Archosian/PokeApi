using System.Text.Json.Serialization;

namespace Sonnet.Models;

/// <summary>
/// Pokémon are the creatures that inhabit the world of the Pokémon games. They can be caught using Pokéballs and
/// trained by battling with other Pokémon. Each Pokémon belongs to a specific species but may take on a variant which
/// makes it differ from other Pokémon of the same species, such as base stats, available abilities and typings.
/// </summary>
public class Pokemon
{
    /// <summary>
    /// The identifier for this resource.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name for this resource.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The base experience gained for defeating this Pokémon.
    /// </summary>
    [JsonPropertyName("base_experience")]
    public int BaseExperience { get; set; }

    /// <summary>
    /// The weight of this Pokémon in hectograms.
    /// </summary>
    public int Weight { get; set; }

    /// <summary>
    /// A set of sprites used to depict this Pokémon in the game.
    /// </summary>
    public Sprites? Sprites { get; set; }

    /// <summary>
    /// (optional) The Flavor Text for this Pokémon.
    /// </summary>
    public string? FlavorText { get; set; }

    /// <summary>
    /// (optional) The Translated Flavor Text for this Pokémon.
    /// </summary>
    public string? TranslatedFlavorText { get; set; }
}