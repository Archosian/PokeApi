using Microsoft.AspNetCore.Mvc;
using Sonnet.Clients.FunTranslation;
using Sonnet.Clients.PokeApi;
using Sonnet.Models;

namespace Sonnet.Controllers;

[ApiController]
[Route("api/pokemon")]
public class PokemonDescriber : ControllerBase
{
    private readonly ILogger<PokemonDescriber> _logger;
    private readonly IPokeApiClient _pokeApiClient;
    private readonly IFunTranslationClient _translationClient;

    public PokemonDescriber(ILogger<PokemonDescriber> logger, IPokeApiClient pokeApiClient, IFunTranslationClient translationClient)
    {
        _logger = logger;
        _pokeApiClient = pokeApiClient;
        _translationClient = translationClient;
    }

    /// <summary>
    /// Fetches Pokémon data, including flavor text. The text will be translated to a format readable by our temporal guest.
    /// Accepts the Pokémon's ID as well as its name.
    /// </summary>
    /// <param name="identifier">The name or id of the pokemon</param>
    /// <param name="skipTranslation">Skip translating the Flavor Text</param>
    /// <param name="token">Cancellation token</param>
    /// <returns></returns>
    [HttpGet("{identifier}")]
    public async Task<ApiResponse<Pokemon>> Get(string identifier, [FromQuery]bool skipTranslation, CancellationToken token)
    {
        var pokemon = await _pokeApiClient.GetByNameOrId(identifier.ToLower(), token);
        if (pokemon == null)
        {
            return ApiResponse<Pokemon>.ErrorResponse($"Could not find Pokemon {identifier}");
        }

        var domainPokemon = Mapping.OfPokeApiPokemon(pokemon);

        var species = pokemon.Species?.Name;
        if (species != null)
        {
            var text = await _pokeApiClient.FlavorTextBySpeciesName(species, token);
            domainPokemon.FlavorText = text;
            if (!skipTranslation)
            {
                domainPokemon.TranslatedFlavorText = await GetFlavorTextForSpecies(species, text, token);
            }
        }

        return ApiResponse<Pokemon>.SuccessResponse(domainPokemon);
    }

    private async Task<string?> GetFlavorTextForSpecies(string species, string?  text, CancellationToken token)
    {
        if (text != null)
        {
            text = await _translationClient.Translate(species, text, token);
        }
        return text;
    }
}