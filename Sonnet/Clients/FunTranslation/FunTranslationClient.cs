using System.Web;
using Microsoft.Extensions.Caching.Distributed;
using RestSharp;
using RestSharp.Serializers;
using Sonnet.Clients.FunTranslation.Models;

namespace Sonnet.Clients.FunTranslation;

public class FunTranslationClient : IFunTranslationClient
{
    private readonly RestClient _client;
    private readonly ILogger<FunTranslationClient> _logger;
    private readonly IDistributedCache _cache;

    public const string BaseUrl = "https://api.funtranslations.com/translate/";
    private const string Key = "translation_";

    private static string CacheKey(string key)
    {
        return $"{Key}{key}";
    }

    /// <summary>
    /// Uses a cache to debounce requests to the strongly rate-limited FunTranslations API.
    /// </summary>
    public FunTranslationClient(ILogger<FunTranslationClient> logger, IDistributedCache cache, RestClient client)
    {
        _client = client;
        _logger = logger;
        _cache = cache;
    }

    public async Task<string?> Translate(string key, string input, CancellationToken token)
    {
        try
        {
            var cachedResponse = await _cache.GetStringAsync(CacheKey(key), token);
            if (!string.IsNullOrEmpty(cachedResponse))
            {
                _logger.LogInformation($"Cache hit on {key}");
                return cachedResponse;
            }
            _logger.LogInformation($"Cache miss, fetching translation for {key}.");

            var request = new RestRequest($"{BaseUrl}shakespeare.json")
                .AddQueryParameter("text", HttpUtility.UrlEncode(input), false)
                .AddStringBody("{}", ContentType.Json);
            var response = await _client.PostAsync<TranslationResponse>(request, token);
            if (response?.Success?.Total == 1 && response.Contents?.Translated != null)
            {
                _logger.LogInformation($"Setting cache for {key} after successful fetch.");
                //TODO Cache with expiry?
                await _cache.SetStringAsync(CacheKey(key), response.Contents.Translated, token);
                return response.Contents.Translated;
            }

            if (response?.Error != null)
            {
                _logger.LogWarning($"The translation API replied with an error: {response.Error.Code} {response.Error.Message}");
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning($"The Translation request failed with an exception: {e.Message}");
        }

        return null;
    }
}