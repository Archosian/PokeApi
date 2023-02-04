namespace Sonnet.Models;

public class Mapping
{
    public static Pokemon OfPokeApiPokemon(Clients.PokeApi.Models.Pokemon poke)
    {
        return new Pokemon
        {
            Id = poke.Id,
            Name = poke.Name,
            BaseExperience = poke.BaseExperience,
            Weight = poke.Weight,
            Sprites = OfPokeApiSprite(poke.Sprites),
            FlavorText = null
        };
    }

    public static Sprites OfPokeApiSprite(Clients.PokeApi.Models.Sprites? sprites)
    {
        return new Sprites
        {
            Front = sprites?.Front,
            Back = sprites?.Back
        };
    }
}