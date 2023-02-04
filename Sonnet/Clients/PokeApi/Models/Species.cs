using System.Text.Json.Serialization;
using Sonnet.Clients.PokeApi.Models;

namespace Sonnet.Clients.Models;

public class Species
{
    [JsonPropertyName("flavor_text_entries")]
    public IEnumerable<FlavorText>? FlavorTexts { get; set; }
}