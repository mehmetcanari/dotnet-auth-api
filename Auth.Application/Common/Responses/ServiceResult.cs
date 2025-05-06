namespace Auth.Application.Common.Responses;

/// <summary>
/// Represents a success or failure result from any service operation.
/// </summary>
public class ServiceResult
{
    public bool IsSuccess { get; protected set; }
    public string? ErrorCode { get; protected set; }
    public string? ErrorMessage { get; protected set; }

    public static ServiceResult Success()
        => new() { IsSuccess = true };

    public static ServiceResult Failure(string code, string message)
        => new() { IsSuccess = false, ErrorCode = code, ErrorMessage = message };
}

/// <summary>
/// Represents a success or failure result from any service operation with a data payload.
/// </summary>
public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; private set; }

    public static ServiceResult<T> Success(T data)
        => new() { IsSuccess = true, Data = data };

    public static ServiceResult<T> Failure(string code, string message)
        => new() { IsSuccess = false, ErrorCode = code, ErrorMessage = message };
}