using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Sonnet.Clients.FunTranslation;
using Moq;
using RestSharp;
using RichardSzalay.MockHttp;

namespace SonnetTests;

public class FunTranslationsClientTests
{

    [Fact]
    public async void IfCachedReturnsValueWithoutClientInteraction()
    {
        //Arrange
        const string cacheKey = "key";
        const string cacheValue = "A value";
        var logger = Mock.Of<ILogger<FunTranslationClient>>();
        var cacheMock = new Mock<IDistributedCache>();
        var mockHandler = new MockHttpMessageHandler();

        var rest = new RestClient(new RestClientOptions{ ConfigureMessageHandler = _ => mockHandler });
        cacheMock.Setup(cache => cache.GetAsync($"translation_{cacheKey}", It.IsAny<CancellationToken>()).Result)
             .Returns(Encoding.ASCII.GetBytes(cacheValue));
        var client = new FunTranslationClient(logger, cacheMock.Object, rest);

        //Act
        var response = await client.Translate(cacheKey, It.IsAny<string>(), new CancellationToken());

        //Assert
        cacheMock.Verify(cache => cache.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        Assert.Equal(response, cacheValue);
    }

    [Fact]
    public async void IfNotCachedCallsClientAndStoresSuccessAndReturns()
    {
        //Arrange
        const string cacheKey = "key";
        const string translationText = "Some particular translation text";
        const string apiResponse = @"{""success"":{""total"":1},""contents"":{""translated"":""Some particular translation text""}}";
        var logger = Mock.Of<ILogger<FunTranslationClient>>();
        var cacheMock = new Mock<IDistributedCache>();
        var mockHandler = new MockHttpMessageHandler();
        mockHandler.When($"{FunTranslationClient.BaseUrl}shakespeare.json")
            .Respond("application/json", apiResponse);

        var rest = new RestClient(new RestClientOptions{ ConfigureMessageHandler = _ => mockHandler });
        cacheMock.Setup(cache => cache.GetAsync($"translation_{cacheKey}", It.IsAny<CancellationToken>()).Result)
            .Returns(Array.Empty<byte>());
        var client = new FunTranslationClient(logger, cacheMock.Object, rest);

        //Act
        var response = await client.Translate(cacheKey, translationText, new CancellationToken());

        //Assert
        cacheMock.Verify(cache => cache.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        cacheMock.Verify(cache =>
            cache.SetAsync($"translation_{cacheKey}",
                           Encoding.ASCII.GetBytes(translationText),
                           It.IsAny<DistributedCacheEntryOptions>(),
                           It.IsAny<CancellationToken>()), Times.Once());
        Assert.Equal(response, translationText);
    }

    [Fact]
    public async void IfNotCachedAndClientBadResponseReturnsWithoutCaching()
    {
        //Arrange
        const string cacheKey = "key";
        const string apiResponse = @"{""error"":{""code"":429,""message"":""Some really bad error message!""}}";
        var logger = Mock.Of<ILogger<FunTranslationClient>>();
        var cacheMock = new Mock<IDistributedCache>();
        var mockHandler = new MockHttpMessageHandler();
        mockHandler.When($"{FunTranslationClient.BaseUrl}/shakespeare.json")
            .Respond("application/json", apiResponse);

        var rest = new RestClient(new RestClientOptions{ ConfigureMessageHandler = _ => mockHandler });
        var client = new FunTranslationClient(logger, cacheMock.Object, rest);

        //Act
        var response = await client.Translate(cacheKey, It.IsAny<string>(), new CancellationToken());

        //Assert
        cacheMock.Verify(cache => cache.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once());
        cacheMock.Verify(cache =>
            cache.SetAsync(It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Never());
        Assert.Null(response);
    }
}