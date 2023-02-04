namespace Sonnet.Clients.FunTranslation;

public interface IFunTranslationClient
{

    /// <summary>
    /// Translate a string of text through the shakespeare.json API. Requires a key to uniquely identify the text, in
    /// order to enable caching. The key is required, since the API has very low rate limits.
    /// </summary>
    /// <param name="key">The unique identifier for the text being sent.</param>
    /// <param name="input">The text being translated.</param>
    /// <param name="token">The CancellationToken.</param>
    /// <returns></returns>
    public Task<string?> Translate(string key, string input, CancellationToken token);
}