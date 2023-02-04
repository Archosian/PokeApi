namespace Sonnet.Clients.FunTranslation.Models;

/// <summary>
/// The contents on a successful Translation response
/// </summary>
public class Contents
{
    /// <summary>
    /// The name of the translation applied
    /// </summary>
    public string? Translation { get; set; }

    /// <summary>
    /// The original text.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// The translated text.
    /// </summary>
    public string? Translated { get; set; }
}