using ExpectedObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sonnet.Clients.FunTranslation;
using Sonnet.Clients.PokeApi;
using Sonnet.Clients.PokeApi.Models;
using Sonnet.Controllers;
using Sonnet.Models;
using Pokemon = Sonnet.Clients.PokeApi.Models.Pokemon;

namespace SonnetTests;

public class PokemonDescriberTests
{
    private static readonly Pokemon examplePokemon = new Pokemon()
    {
        BaseExperience = 0,
        Id = 0,
        Name = "dummyPokemon",
        Species = new NamedApiResource { Name = "specialspecies" },
        Sprites = null,
        Weight = 0
    };

    [Fact]
    public async void GetReturnsPokemonIfNoSpecies()
    {
        //Arrange
        var pokemon = examplePokemon;
        pokemon.Species = null;
        var mockPokeApi = new Mock<IPokeApiClient>();
        mockPokeApi
            .Setup(api => api.GetByNameOrId(pokemon.Name.ToLower(), It.IsAny<CancellationToken>()).Result)
            .Returns(pokemon);
        var mockTranslationApi = new Mock<IFunTranslationClient>();

        var describer = new PokemonDescriber(mockPokeApi.Object, mockTranslationApi.Object);

        //Act
        var response = await describer.Get(examplePokemon.Name, true, new CancellationToken());

        //Assert
        var result = response as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        ApiResponse<Sonnet.Models.Pokemon>
            .SuccessResponse(Mapping.OfPokeApiPokemon(pokemon))
            .ToExpectedObject().Equals(result.Value);
        //TODO this comparison isn't great: It won't catch that we aren't using the Domain model!
    }

    [Fact]
    public async void GetReturnsPokemonWithoutTranslationIfSkipping()
    {
        //Arrange
        var pokemon = Mapping.OfPokeApiPokemon(examplePokemon);
        var flavorText = "My favourite flavor text";
        pokemon.FlavorText = flavorText;
        var mockPokeApi = new Mock<IPokeApiClient>();
        mockPokeApi
            .Setup(api => api.GetByNameOrId(examplePokemon.Name.ToLower(), It.IsAny<CancellationToken>()).Result)
            .Returns(examplePokemon);
        mockPokeApi
            .Setup(api => api.FlavorTextBySpeciesName(examplePokemon.Species.Name, It.IsAny<CancellationToken>()).Result)
            .Returns(flavorText);
        var mockTranslationApi = new Mock<IFunTranslationClient>();
        var describer = new PokemonDescriber(mockPokeApi.Object, mockTranslationApi.Object);

        //Act
        var response = await describer.Get(examplePokemon.Name, true, new CancellationToken());

        //Assert
        mockPokeApi.Verify(api => api.GetByNameOrId(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        mockPokeApi.Verify(api => api.FlavorTextBySpeciesName(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        mockTranslationApi.Verify(api => api.Translate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
        var result = response as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        ApiResponse<Sonnet.Models.Pokemon>
            .SuccessResponse(pokemon)
            .ToExpectedObject().Equals(result.Value);
    }

    [Fact]
    public async void GetReturnsPokemonWithTranslation()
    {
        //Arrange
        var pokemon = Mapping.OfPokeApiPokemon(examplePokemon);
        var flavorText = "My favourite flavor text";
        var translation = "My most bestest smörgårdsbord of text";
        pokemon.FlavorText = flavorText;
        pokemon.TranslatedFlavorText = translation;
        var mockPokeApi = new Mock<IPokeApiClient>();
        mockPokeApi
            .Setup(api => api.GetByNameOrId(examplePokemon.Name.ToLower(), It.IsAny<CancellationToken>()).Result)
            .Returns(examplePokemon);
        mockPokeApi
            .Setup(api => api.FlavorTextBySpeciesName(examplePokemon.Species.Name, It.IsAny<CancellationToken>()).Result)
            .Returns(flavorText);
        var mockTranslationApi = new Mock<IFunTranslationClient>();
        mockTranslationApi
            .Setup(api => api.Translate(examplePokemon.Species.Name, flavorText, It.IsAny<CancellationToken>()).Result)
            .Returns(translation);
        var describer = new PokemonDescriber(mockPokeApi.Object, mockTranslationApi.Object);

        //Act
        var response = await describer.Get(examplePokemon.Name, true, new CancellationToken());

        //Assert
        mockPokeApi.Verify(api => api.GetByNameOrId(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        mockPokeApi.Verify(api => api.FlavorTextBySpeciesName(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        mockTranslationApi.Verify(api => api.Translate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
        var result = response as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
        ApiResponse<Sonnet.Models.Pokemon>
            .SuccessResponse(pokemon)
            .ToExpectedObject().Equals(result.Value);
    }

    [Fact]
    public async void GetReturnsErrorIfNotFound()
    {
        //Arrange
        var pokemon = examplePokemon;
        pokemon.Species = null;
        var mockPokeApi = new Mock<IPokeApiClient>();
        var mockTranslationApi = new Mock<IFunTranslationClient>();
        var describer = new PokemonDescriber(mockPokeApi.Object, mockTranslationApi.Object);

        //Act
        var response = await describer.Get(examplePokemon.Name, true, new CancellationToken());

        //Assert
        mockPokeApi.Verify(api => api.GetByNameOrId(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        var result = response as NotFoundObjectResult;
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }
}