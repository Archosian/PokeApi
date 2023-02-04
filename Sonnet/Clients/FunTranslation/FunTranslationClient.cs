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

    private const string Key = "translation_";

    private static string CacheKey(string key)
    {
        return $"{Key}{key}";
    }

    /// <summary>
    /// Uses a cache to debounce requests to the strongly rate-limited FunTranslations API.
    /// </summary>
    public FunTranslationClient(ILogger<FunTranslationClient> logger, IDistributedCache cache)
    {
        _client = new RestClient(Environment.GetEnvironmentVariable("FUN_TRANSLATION_BASE_URL")!);
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

            var request = new RestRequest("shakespeare.json")
                .AddQueryParameter("text", HttpUtility.UrlEncode(input), false)
                .AddStringBody("{}", ContentType.Json);
            var response = await _client.PostAsync<TranslationResponse>(request, token);
            if (response?.Success?.Total == 1 && response.Contents?.Translated != null)
            {
                //TODO Cache with no expiry.
                await _cache.SetStringAsync(CacheKey(key), response.Contents.Translated, token);
                return response.Contents.Translated;
            }

            if (response?.Error?.Code == 429)
            {
                _logger.LogWarning("The translation API is rate limiting requests");
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning($"The Translation request failed with an exception: {e.Message}");
        }

        return null;
    }
}