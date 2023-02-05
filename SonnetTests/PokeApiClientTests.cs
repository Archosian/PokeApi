using System.Text.Json;
using ExpectedObjects;
using Microsoft.Extensions.Logging;
using Moq;
using RestSharp;
using RichardSzalay.MockHttp;
using Sonnet.Clients.PokeApi;
using Sonnet.Clients.PokeApi.Models;

namespace SonnetTests;

public class PokeApiClientTests
{
    private static readonly string PokeName = "dummyPoke";
    private static readonly Pokemon DummyPoke = new Pokemon()
    {
        BaseExperience = 0,
        Id = 0,
        Name = PokeName,
        Species = new NamedApiResource() { Name = "specialspecies" },
        Sprites = null,
        Weight = 0
    };



    [Fact]
    public async void GetByNameUsesReponseFromClient()
    {
        //Arrange
        var logger = Mock.Of<ILogger<PokeApiClient>>();
        var mockHandler = new MockHttpMessageHandler();
        mockHandler.Expect($"{PokeApiClient.BaseUrl}pokemon/{DummyPoke.Name}")
            .Respond("application/json", JsonSerializer.Serialize(DummyPoke));
        var rest = new RestClient(new RestClientOptions{ ConfigureMessageHandler = _ => mockHandler });
        var client = new PokeApiClient(logger, rest);

        //Act
        var response = await client.GetByNameOrId(PokeName, new CancellationToken());

        //Assert
        mockHandler.VerifyNoOutstandingExpectation();
        DummyPoke.ToExpectedObject().ShouldEqual(response);
    }

    [Fact]
    public async void GetByNameReturnsNullOnExceptions()
    {
        //Arrange
        var logger = Mock.Of<ILogger<PokeApiClient>>();
        var mockHandler = new MockHttpMessageHandler();
        mockHandler.Fallback.Throw(new Exception("Intentionally threw"));

        var rest = new RestClient(new RestClientOptions{ ConfigureMessageHandler = _ => mockHandler });
        var client = new PokeApiClient(logger, rest);

        //Act
        var response = await client.GetByNameOrId(PokeName, new CancellationToken());

        //Assert
        Assert.Null(response);
    }

    [Fact]
    public async void FlavorTextBySpeciesRetrievesExpectedEntry()
    {
        //Arrange
        var testName = "pokeName";
        var expectedText = "Wow this is some unique output right here";
        var dummySpecies = new Species()
        {
            FlavorTextEntries = new List<FlavorText>()
            {
                new FlavorText { Text = "Some data", Version = new NamedApiResource { Name = "wrong version"}},
                new FlavorText { Text = expectedText, Version = new NamedApiResource { Name = PokeApiClient.TargetVersion}}
            }
        };
        await TestExpectationGetFlavorText(testName, dummySpecies, expectedText);
    }

    [Fact]
    public async void FlavorTextBySpeciesReturnsEmptyStringIfNoEntryFound()
    {
        //Arrange
        var testName = "pokeName";
        var dummySpecies = new Species()
        {
            FlavorTextEntries = new List<FlavorText>()
            {
                new FlavorText { Text = "Some data", Version = new NamedApiResource { Name = "wrong version"}},
                new FlavorText { Text = "Other data", Version = new NamedApiResource { Name = "wrong version"}}
            }
        };
        await TestExpectationGetFlavorText(testName, dummySpecies, string.Empty);
    }

    [Fact]
    public async void FlavorTextBySpeciesReturnsEmptyStringIfListEmpty()
    {
        //Arrange
        var testName = "pokeName";
        var dummySpecies = new Species()
        {
            FlavorTextEntries = new List<FlavorText>()
        };
        await TestExpectationGetFlavorText(testName, dummySpecies, string.Empty);
    }

    private static async Task TestExpectationGetFlavorText(string name, Species expectedData, string expectedOutput)
    {
        //Arrange
        var logger = Mock.Of<ILogger<PokeApiClient>>();
        var mockHandler = new MockHttpMessageHandler();
        mockHandler.Expect($"{PokeApiClient.BaseUrl}pokemon-species/{name}")
            .Respond("application/json", JsonSerializer.Serialize(expectedData));

        var rest = new RestClient(new RestClientOptions{ ConfigureMessageHandler = _ => mockHandler });
        var client = new PokeApiClient(logger, rest);

        //Act
        var response = await client.FlavorTextBySpeciesName(name, new CancellationToken());

        //Assert
        mockHandler.VerifyNoOutstandingExpectation();
        Assert.Equal(response, expectedOutput);
    }
}