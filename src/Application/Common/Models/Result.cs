using System.Net;

namespace Application.Common.Models;

/// <summary>
/// Represents the result of an operation with success/failure state and optional data
/// </summary>
/// <typeparam name="T">The type of data returned on success</typeparam>
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public T? Data { get; private set; }
    public string? Message { get; private set; }
    public IEnumerable<string> Errors { get; private set; } = Array.Empty<string>();
    public HttpStatusCode StatusCode { get; private set; } = HttpStatusCode.OK;
    public string? ErrorCode { get; private set; }

    protected Result(bool isSuccess, T? data, string? message, IEnumerable<string>? errors = null, 
                  HttpStatusCode statusCode = HttpStatusCode.OK, string? errorCode = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        Message = message;
        Errors = errors ?? Array.Empty<string>();
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates a successful result with data
    /// </summary>
    public static Result<T> Success(T data, string? message = null)
    {
        return new Result<T>(true, data, message, null, HttpStatusCode.OK);
    }

    /// <summary>
    /// Creates a successful result without data
    /// </summary>
    public static Result<T> Success(string? message = null)
    {
        return new Result<T>(true, default, message, null, HttpStatusCode.OK);
    }

    /// <summary>
    /// Creates a failed result with a single error message
    /// </summary>
    public static Result<T> Failure(string error, HttpStatusCode statusCode = HttpStatusCode.BadRequest, string? errorCode = null)
    {
        return new Result<T>(false, default, null, new[] { error }, statusCode, errorCode);
    }

    /// <summary>
    /// Creates a failed result with multiple error messages
    /// </summary>
    public static Result<T> Failure(IEnumerable<string> errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest, string? errorCode = null)
    {
        return new Result<T>(false, default, null, errors, statusCode, errorCode);
    }

    /// <summary>
    /// Creates a not found result
    /// </summary>
    public static Result<T> NotFound(string? message = null)
    {
        return new Result<T>(false, default, message, null, HttpStatusCode.NotFound, "NOT_FOUND");
    }

    /// <summary>
    /// Creates an unauthorized result
    /// </summary>
    public static Result<T> Unauthorized(string? message = null)
    {
        return new Result<T>(false, default, message, null, HttpStatusCode.Unauthorized, "UNAUTHORIZED");
    }

    /// <summary>
    /// Creates a forbidden result
    /// </summary>
    public static Result<T> Forbidden(string? message = null)
    {
        return new Result<T>(false, default, message, null, HttpStatusCode.Forbidden, "FORBIDDEN");
    }

    /// <summary>
    /// Creates a conflict result
    /// </summary>
    public static Result<T> Conflict(string? message = null)
    {
        return new Result<T>(false, default, message, null, HttpStatusCode.Conflict, "CONFLICT");
    }

    /// <summary>
    /// Creates a validation error result
    /// </summary>
    public static Result<T> ValidationError(IEnumerable<string> errors)
    {
        return new Result<T>(false, default, "Validation failed", errors, HttpStatusCode.BadRequest, "VALIDATION_ERROR");
    }

    /// <summary>
    /// Creates an internal server error result
    /// </summary>
    public static Result<T> InternalError(string? message = null)
    {
        return new Result<T>(false, default, message ?? "An internal server error occurred", 
                           null, HttpStatusCode.InternalServerError, "INTERNAL_ERROR");
    }

    /// <summary>
    /// Implicitly converts data to a successful result
    /// </summary>
    public static implicit operator Result<T>(T data)
    {
        return Success(data);
    }

    /// <summary>
    /// Maps the result data to a different type
    /// </summary>
    public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        if (IsFailure)
        {
            return new Result<TNew>(false, default, Message, Errors, StatusCode, ErrorCode);
        }

        try
        {
            var newData = Data != null ? mapper(Data) : default;
            return Result<TNew>.Success(newData, Message);
        }
        catch (Exception ex)
        {
            return Result<TNew>.InternalError($"Mapping failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Combines this result with another result
    /// </summary>
    public Result<T> Combine(Result<T> other)
    {
        if (IsFailure)
            return this;
        
        return other;
    }
}

/// <summary>
/// Non-generic result for operations that don't return data
/// </summary>
public class Result : Result<object>
{
    private Result(bool isSuccess, string? message, IEnumerable<string>? errors = null, 
                  HttpStatusCode statusCode = HttpStatusCode.OK, string? errorCode = null)
        : base(isSuccess, null, message, errors, statusCode, errorCode)
    {
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public new static Result Success(string? message = null)
    {
        return new Result(true, message);
    }

    /// <summary>
    /// Creates a failed result with a single error message
    /// </summary>
    public new static Result Failure(string error, HttpStatusCode statusCode = HttpStatusCode.BadRequest, string? errorCode = null)
    {
        return new Result(false, null, new[] { error }, statusCode, errorCode);
    }

    /// <summary>
    /// Creates a failed result with multiple error messages
    /// </summary>
    public new static Result Failure(IEnumerable<string> errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest, string? errorCode = null)
    {
        return new Result(false, null, errors, statusCode, errorCode);
    }

    /// <summary>
    /// Creates a not found result
    /// </summary>
    public new static Result NotFound(string? message = null)
    {
        return new Result(false, message, null, HttpStatusCode.NotFound, "NOT_FOUND");
    }

    /// <summary>
    /// Creates an unauthorized result
    /// </summary>
    public new static Result Unauthorized(string? message = null)
    {
        return new Result(false, message, null, HttpStatusCode.Unauthorized, "UNAUTHORIZED");
    }

    /// <summary>
    /// Creates a forbidden result
    /// </summary>
    public new static Result Forbidden(string? message = null)
    {
        return new Result(false, message, null, HttpStatusCode.Forbidden, "FORBIDDEN");
    }

    /// <summary>
    /// Creates a conflict result
    /// </summary>
    public new static Result Conflict(string? message = null)
    {
        return new Result(false, message, null, HttpStatusCode.Conflict, "CONFLICT");
    }

    /// <summary>
    /// Creates a validation error result
    /// </summary>
    public new static Result ValidationError(IEnumerable<string> errors)
    {
        return new Result(false, "Validation failed", errors, HttpStatusCode.BadRequest, "VALIDATION_ERROR");
    }

    /// <summary>
    /// Creates an internal server error result
    /// </summary>
    public new static Result InternalError(string? message = null)
    {
        return new Result(false, message ?? "An internal server error occurred", 
                         null, HttpStatusCode.InternalServerError, "INTERNAL_ERROR");
    }
}
