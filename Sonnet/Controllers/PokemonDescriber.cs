using Microsoft.AspNetCore.Mvc;
using Sonnet.Clients.PokeApi;
using Sonnet.Models;

namespace Sonnet.Controllers;

[ApiController]
[Route("api/pokemon")]
public class PokemonDescriber : ControllerBase
{
    private readonly ILogger<PokemonDescriber> _logger;
    private readonly PokeApiClient _pokeApiClient;

    public PokemonDescriber(ILogger<PokemonDescriber> logger, PokeApiClient pokeApiClient)
    {
        _logger = logger;
        _pokeApiClient = pokeApiClient;
    }

    [HttpGet("{name}")]
    public async Task<ApiResponse<Pokemon>> Get(string name, CancellationToken token)
    {
        _logger.Log(LogLevel.Information, $"GET pokemon by name: {name}");

        var pmon = await _pokeApiClient.GetByName(name, token);
        if (pmon == null)
        {
            return ApiResponse<Pokemon>.ErrorResponse($"Could not find Pokemon {name}");
        }

        return ApiResponse<Pokemon>.SuccessResponse(Mapping.OfPokeApiPokemon(pmon));
    }
}