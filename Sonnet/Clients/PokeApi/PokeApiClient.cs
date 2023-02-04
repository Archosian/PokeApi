using RestSharp;
using Sonnet.Clients.PokeApi.Models;

namespace Sonnet.Clients.PokeApi;

public class PokeApiClient : RestClient
{
    private readonly RestClient _client;

    public PokeApiClient()
    {
        _client = new RestClient(Environment.GetEnvironmentVariable("POKE_API_BASE_URL")!);
    }

    public async Task<Pokemon?> GetByName(string name, CancellationToken token)
    {
        return await _client.GetJsonAsync<Pokemon>($"/pokemon/{name}", token);
    } //todo error or notfound handling?

    public string FlavorTextForPokemon(string name)
    {
        return "";
    }
}