namespace BuildingBlocks.Core.Response;

public class ApiResponse<TData>
{
    public bool Succeeded { get; set; }

    public TData? Data { get; set; }

    public string? Message { get; set; }

    public ApiResponse()
    {
        Succeeded = false;
        Data = default;
        Message = string.Empty;
    }

    internal ApiResponse(TData? data, bool succeeded, string? message)
    {
        Succeeded = succeeded;
        Data = data;
        Message = message;
    }

    public static ApiResponse<TData> Success(TData data, string? message = null)
    {
        return new ApiResponse<TData>(data, true, message);
    }

    public static ApiResponse<TData> Error(TData data, string message)
    {
        return new ApiResponse<TData>(data, false, message);
    }

    public static ApiResponse<TData> Error(string message)
    {
        return new ApiResponse<TData>(default, false, message);
    }
}

public class ApiResponse
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }

    public ApiResponse()
    {
        Succeeded = false;
        Message = string.Empty;
    }

    internal ApiResponse(bool succeeded, string? message)
    {
        Succeeded = succeeded;
        Message = message;
    }

    public static ApiResponse Success(string? message = null)
    {
        return new ApiResponse(true, message);
    }

    public static ApiResponse Error(string message)
    {
        return new ApiResponse(false, message);
    }
}

