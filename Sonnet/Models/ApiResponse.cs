namespace Sonnet.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Response { get; set; }
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