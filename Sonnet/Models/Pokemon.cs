using System.Text.Json.Serialization;

namespace Sonnet.Models;

public class Pokemon
{
    public int Id { get; set; }
    public string? Name { get; set; }
    [JsonPropertyName("base_experience")]
    public int BaseExperience { get; set; }
    public int Weight { get; set; }
    public Sprites? Sprites { get; set; }
    public string? FlavorText { get; set; }
}