using Sonnet.Clients.PokeApi.Models;

namespace Sonnet.Clients.PokeApi;

public interface IPokeApiClient
{
    /// <summary>
    /// Get a Pokémon by its name, or by its PokeApi identifier.
    /// </summary>
    /// <param name="identifier">The name or identifier of the Pokémon.</param>
    /// <param name="token">The CancellationToken.</param>
    /// <returns></returns>
    public Task<Pokemon?> GetByNameOrId(string identifier, CancellationToken token);

    /// <summary>
    /// The FlavorText for a given species.
    /// </summary>
    /// <param name="name">The name of the Pokémon species.</param>
    /// <param name="token">The CancellationToken.</param>
    /// <returns></returns>
    public Task<string?> FlavorTextBySpeciesName(string name, CancellationToken token);
}