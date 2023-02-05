using ExpectedObjects;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sonnet.Clients.FunTranslation;
using Sonnet.Clients.PokeApi;
using Sonnet.Clients.PokeApi.Models;
using Sonnet.Controllers;
using Sonnet.Models;
using PokeApiPokemon = Sonnet.Clients.PokeApi.Models.Pokemon;

namespace SonnetTests;

public class PokemonDescriberTests
{
    private static readonly string SpeciesName = "specialspecies";
    private static PokeApiPokemon ExamplePokemon(string name)
    {
        return new PokeApiPokemon
            {
                BaseExperience = 0,
                Id = 0,
                Name = name,
                Species = new NamedApiResource { Name = SpeciesName },
                Sprites = null,
                Weight = 0
            };
    }

    [Fact]
    public async void GetReturnsPokemonIfNoSpecies()
    {
        //Arrange
        var pokeName = "dummyPokemon";
        var pokemon = ExamplePokemon(pokeName);
        pokemon.Species = null;
        var mockPokeApi = new Mock<IPokeApiClient>();
        mockPokeApi
            .Setup(api => api.GetByNameOrId(pokeName.ToLower(), It.IsAny<CancellationToken>()).Result)
            .Returns(pokemon);
        var mockTranslationApi = new Mock<IFunTranslationClient>();

        var describer = new PokemonDescriber(mockPokeApi.Object, mockTranslationApi.Object);

        //Act
        var response = await describer.Get(pokeName, true, new CancellationToken());

        //Assert
        var result = response as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        Assert.NotNull(result.Value);
        Assert.True(ApiResponse<Sonnet.Models.Pokemon>
                        .SuccessResponse(Mapping.OfPokeApiPokemon(pokemon))
                        .ToExpectedObject().Equals(result.Value));
        //TODO this comparison tool isn't great: It won't catch if we aren't using the Domain model!
    }

    [Fact]
    public async void GetReturnsPokemonWithoutTranslationIfSkipping()
    {
        //Arrange
        var ct = new CancellationToken();
        var pokeName = "dummyPokemon";
        var examplePokemon = ExamplePokemon(pokeName);
        var pokemon = Mapping.OfPokeApiPokemon(examplePokemon);
        var flavorText = "My favourite flavor text";
        pokemon.FlavorText = flavorText;
        var mockPokeApi = new Mock<IPokeApiClient>();
        mockPokeApi
            .Setup(api => api.GetByNameOrId(pokeName.ToLower(), ct).Result)
            .Returns(examplePokemon);
        mockPokeApi
            .Setup(api => api.FlavorTextBySpeciesName(SpeciesName, ct).Result)
            .Returns(flavorText);
        var mockTranslationApi = new Mock<IFunTranslationClient>();
        var describer = new PokemonDescriber(mockPokeApi.Object, mockTranslationApi.Object);

        //Act
        var response = await describer.Get(pokeName, true, ct);

        //Assert
        mockPokeApi.Verify(api => api.GetByNameOrId(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        mockPokeApi.Verify(api => api.FlavorTextBySpeciesName(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        mockTranslationApi.Verify(api => api.Translate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never());
        var result = response as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        Assert.NotNull(result.Value);
        Assert.True(ApiResponse<Sonnet.Models.Pokemon>
                        .SuccessResponse(pokemon)
                        .ToExpectedObject().Equals(result.Value));
    }

    [Fact]
    public async void GetReturnsPokemonWithTranslation()
    {
        //Arrange
        var pokeName = "dummyPokemon";
        var examplePokemon = ExamplePokemon(pokeName);
        var pokemon = Mapping.OfPokeApiPokemon(examplePokemon);
        var flavorText = "My favourite flavor text";
        var translation = "My most bestest smörgårdsbord of text";
        pokemon.FlavorText = flavorText;
        pokemon.TranslatedFlavorText = translation;
        var mockPokeApi = new Mock<IPokeApiClient>();
        mockPokeApi
            .Setup(api => api.GetByNameOrId(pokeName.ToLower(), It.IsAny<CancellationToken>()).Result)
            .Returns(examplePokemon);
        mockPokeApi
            .Setup(api => api.FlavorTextBySpeciesName(SpeciesName, It.IsAny<CancellationToken>()).Result)
            .Returns(flavorText);

        var mockTranslationApi = new Mock<IFunTranslationClient>();
        mockTranslationApi
            .Setup(api => api.Translate(SpeciesName, flavorText, It.IsAny<CancellationToken>()).Result)
            .Returns(translation);
        var describer = new PokemonDescriber(mockPokeApi.Object, mockTranslationApi.Object);

        //Act
        var response = await describer.Get(pokeName, false, new CancellationToken());

        //Assert
        mockPokeApi.Verify(api => api.GetByNameOrId(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        mockPokeApi.Verify(api => api.FlavorTextBySpeciesName(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        mockTranslationApi.Verify(api => api.Translate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        var result = response as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        Assert.NotNull(result.Value);
        Assert.True(ApiResponse<Sonnet.Models.Pokemon>
                    .SuccessResponse(pokemon)
                    .ToExpectedObject().Equals(result.Value));
    }

    [Fact]
    public async void GetReturnsErrorIfNotFound()
    {
        //Arrange
        var pokeName = "dummyPokemon";
        var mockPokeApi = new Mock<IPokeApiClient>();
        var mockTranslationApi = new Mock<IFunTranslationClient>();
        var describer = new PokemonDescriber(mockPokeApi.Object, mockTranslationApi.Object);

        //Act
        var response = await describer.Get(pokeName, true, new CancellationToken());

        //Assert
        mockPokeApi.Verify(api => api.GetByNameOrId(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        var result = response as NotFoundObjectResult;
        Assert.NotNull(result);
        Assert.Equal(404, result!.StatusCode);
    }
}