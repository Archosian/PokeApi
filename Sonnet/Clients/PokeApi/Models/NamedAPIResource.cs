namespace Sonnet.Clients.PokeApi.Models;

/// <summary>
/// An API type to reference the name and the redirect URL of a resource.
/// </summary>
public class NamedApiResource
{
    /// <summary>
    /// The name of the referenced resource.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The Redirect (PokeAPI) redirect URL of the referenced resource.
    /// </summary>
    public string? Url { get; set; }
}