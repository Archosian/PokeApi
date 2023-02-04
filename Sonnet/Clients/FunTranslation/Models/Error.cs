namespace Sonnet.Clients.FunTranslation.Models;

/// <summary>
/// The contents of a non successful request.
/// </summary>
public class Error
{
    /// <summary>
    /// The error/status code of the error.
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// The message of the error.
    /// </summary>
    public string? Message { get; set; }
}