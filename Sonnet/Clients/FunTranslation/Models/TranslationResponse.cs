namespace Sonnet.Clients.FunTranslation.Models;

/// <summary>
/// The contents of a Translation response.
/// </summary>
public class TranslationResponse
{
    /// <summary>
    /// If the response is successful, contains a count.
    /// </summary>
    public Success? Success { get; set; }

    /// <summary>
    /// If the response was not successful, will be set.
    /// </summary>
    public Error? Error { get; set; }

    /// <summary>
    /// If the response is successful, will be set with the contents of the response.
    /// </summary>
    public Contents? Contents { get; set; }
}