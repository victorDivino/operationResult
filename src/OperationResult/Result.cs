using System.Diagnostics.CodeAnalysis;

namespace OperationResult;

/// <summary>
/// The result of an operation that returns no value: either <see cref="Ok"/> or an <see cref="System.Exception"/>.
/// </summary>
/// <remarks>
/// <c>default(Result)</c> holds neither case (its <c>Value</c> is <see langword="null"/>), so it is not a success.
/// Add a <c>null</c> arm when switching over results that may be default.
/// </remarks>
public union Result(Ok, Exception) : IResult<Result>
{
    public Exception? Exception => Value as Exception;

    [MemberNotNullWhen(false, nameof(Exception))]
    public bool IsSuccess => Value is Ok;

    public bool ErrorIs<TException>()
        where TException : Exception
        => Value is TException;

    public static Result Success()
        => new(Ok.Instance);

    public static Result Error(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        return new(exception);
    }

    public static Result<T> Success<T>(T value)
        => Result<T>.Success(value);

    public static Result<T> Error<T>(Exception exception)
        => Result<T>.Error(exception);

    // Kept as user-defined conversions so consumers on C# 14 or earlier can still write `return exception;`.
    public static implicit operator Result(Exception exception)
        => Error(exception);

    public static implicit operator bool(Result result)
        => result.IsSuccess;

    public static implicit operator Task<Result>(Result result)
        => result.AsTask;

    public void Deconstruct(out bool success, out Exception? exception)
        => (success, exception) = (IsSuccess, Exception);

    public Task<Result> AsTask => Task.FromResult(this);

    public override string ToString() => Value?.ToString() ?? string.Empty;
}
