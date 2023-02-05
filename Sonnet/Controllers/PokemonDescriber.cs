using System.Net;
using Microsoft.AspNetCore.Mvc;
using Sonnet.Clients.FunTranslation;
using Sonnet.Clients.PokeApi;
using Sonnet.Models;

namespace Sonnet.Controllers;

[ApiController]
[Route("api/pokemon")]
public class PokemonDescriber : ControllerBase
{
    private readonly IPokeApiClient _pokeApiClient;
    private readonly IFunTranslationClient _translationClient;

    public PokemonDescriber(IPokeApiClient pokeApiClient, IFunTranslationClient translationClient)
    {
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
    [ProducesResponseType(typeof(ApiResponse<Pokemon>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<Pokemon>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Get(string identifier, [FromQuery]bool skipTranslation, CancellationToken token)
    {
        var pokemon = await _pokeApiClient.GetByNameOrId(identifier.ToLower(), token);
        if (pokemon == null)
        {
            return NotFound(ApiResponse<Pokemon>.ErrorResponse($"Could not find Pokemon {identifier}"));
        }

        var domainPokemon = Mapping.OfPokeApiPokemon(pokemon);

        var species = pokemon.Species?.Name;
        if (species != null)
        {
            var text = await _pokeApiClient.FlavorTextBySpeciesName(species, token);
            domainPokemon.FlavorText = text;
            if (!skipTranslation && text != null)
            {
                domainPokemon.TranslatedFlavorText = await _translationClient.Translate(species, text, token);
            }
        }

        return Ok(ApiResponse<Pokemon>.SuccessResponse(domainPokemon));
    }
}