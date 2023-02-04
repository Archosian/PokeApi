using System.Text.RegularExpressions;
using RestSharp;
using Sonnet.Clients.PokeApi.Models;

namespace Sonnet.Clients.PokeApi;

public class PokeApiClient : IPokeApiClient
{
    private readonly RestClient _client;
    private readonly ILogger<PokeApiClient> _logger;

    private const string TargetVersion = "ruby";

    public PokeApiClient(ILogger<PokeApiClient> logger)
    {
        _client = new RestClient(Environment.GetEnvironmentVariable("POKE_API_BASE_URL")!);
        _logger = logger;
    }

    public async Task<Pokemon?> GetByNameOrId(string identifier, CancellationToken token)
    {
        try
        {
            return await _client.GetJsonAsync<Pokemon>($"/pokemon/{identifier}", token);
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> FlavorTextBySpeciesName(string name, CancellationToken token)
    {
        try
        {
            _logger.LogInformation($"Fetching species {name}");
            var species = await _client.GetJsonAsync<Species>($"/pokemon-species/{name}", token);
            var text = species?.FlavorTextEntries?
                            .Where(ft => ft.Version?.Name?.Equals(TargetVersion) ?? false)
                            .FirstOrDefault();
            return SanitizeRawText(text?.Text ?? string.Empty);
        }
        catch
        {
            _logger.LogWarning($"Failed to fetch species {name}");
            return null;
        }
    }

    private static string SanitizeRawText(string text)
    {
        var pattern = new Regex("[\n\f]");
        return pattern.Replace(text, " ");
    }
}