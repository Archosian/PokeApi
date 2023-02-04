namespace Sonnet.Models;

/// <summary>
/// A structural wrapper type for all replies through Sonnet.
/// </summary>
/// <typeparam name="T">The type of the object being wrapped.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Whether the response is successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The response contents, when successful.
    /// </summary>
    public T? Response { get; set; }

    /// <summary>
    /// An error message, when the response is not successful.
    /// </summary>
    public string? Error { get; set; }

    public static ApiResponse<T> SuccessResponse(T t)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Response = t
        };
    }

    public static ApiResponse<T> ErrorResponse(string error)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = error
        };
    }
}