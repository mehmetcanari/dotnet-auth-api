namespace Auth.Application.Common.Responses;

/// <summary>
/// Represents a success or failure result from any service operation.
/// </summary>
public class Result
{
    public bool IsSuccess { get; protected set; }
    public string? ErrorCode { get; protected set; }
    public string? ErrorMessage { get; protected set; }

    public static Result Success()
        => new() { IsSuccess = true };

    public static Result Failure(string code, string message)
        => new() { IsSuccess = false, ErrorCode = code, ErrorMessage = message };
}

/// <summary>
/// Represents a success or failure result from any service operation with a data payload.
/// </summary>
public class Result<T> : Result
{
    public T? Data { get; private set; }

    public static Result<T> Success(T data)
        => new() { IsSuccess = true, Data = data };

    public static Result<T> Failure(string code, string message)
        => new() { IsSuccess = false, ErrorCode = code, ErrorMessage = message };
}