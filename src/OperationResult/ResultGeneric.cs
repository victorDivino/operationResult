using System.Diagnostics.CodeAnalysis;

namespace OperationResult;

/// <summary>
/// The result of an operation that returns a <typeparamref name="T"/>: either the value or an <see cref="System.Exception"/>.
/// </summary>
/// <remarks>
/// <para>Switch over it exhaustively: <c>result switch { T value => ..., Exception error => ..., null => ... }</c>.</para>
/// <para><c>default(Result&lt;T&gt;)</c> holds neither case and is not a success. Successful values cannot be <see langword="null"/>.</para>
/// <para><typeparamref name="T"/> must not be <see cref="System.Exception"/> or a type it converts to, or the cases become ambiguous.</para>
/// </remarks>
public union Result<T>(T, Exception) : IResultValue<T, Result<T>>
{
    public Exception? Exception => Value as Exception;

    [MemberNotNullWhen(false, nameof(Exception))]
    public bool IsSuccess => Value is T and not System.Exception;

    T? IResultValue<T, Result<T>>.Value => GetValueOrDefault();

    /// <summary>Gets the successful value, if any.</summary>
    public bool TryGetValue([MaybeNullWhen(false)] out T value)
    {
        if (IsSuccess)
        {
            value = (T)Value!;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>Returns the successful value, or <c>default</c> when the result is an error or empty.</summary>
    public T? GetValueOrDefault()
        => TryGetValue(out var value) ? value : default;

    /// <summary>Returns the successful value, or <paramref name="defaultValue"/> when the result is an error or empty.</summary>
    public T GetValueOrDefault(T defaultValue)
        => TryGetValue(out var value) ? value : defaultValue;

    public bool ErrorIs<TException>()
        where TException : Exception
        => Value is TException;

    public static Result<T> Success(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(value);
    }

    public static Result<T> Error(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        return new(exception);
    }

    // Kept as user-defined conversions so consumers on C# 14 or earlier can still write `return value;` / `return exception;`.
    public static implicit operator Result<T>(T value)
        => Success(value);

    public static implicit operator Result<T>(Exception exception)
        => Error(exception);

    public Result<TEnd> ChangeInAnotherResult<TEnd>(Func<T, TEnd> converter)
        where TEnd : IResult<TEnd>
        => TryGetValue(out var value)
            ? Result<TEnd>.Success(converter(value))
            : Result<TEnd>.Error(Exception!);

    public Result ChangeInNoResult()
        => IsSuccess
            ? Result.Success()
            : Result.Error(Exception);

    public void Deconstruct(out bool success, out T? value)
        => (success, value) = (IsSuccess, GetValueOrDefault());

    public void Deconstruct(out bool success, out T? value, out Exception? exception)
        => (success, value, exception) = (IsSuccess, GetValueOrDefault(), Exception);

    static Result<T> IResultValue<T, Result<T>>.Success(T value) => Success(value);

    static Result<T> IResult<Result<T>>.Success() => throw new NotSupportedException("Result<T> needs a value. Use Result<T>.Success(T).");

    static Result<T> IResult<Result<T>>.Error(Exception error) => Error(error);

    public static implicit operator bool(Result<T> result)
        => result.IsSuccess;

    public static implicit operator Task<Result<T>>(Result<T> result)
        => result.AsTask;

    public Task<Result<T>> AsTask => Task.FromResult(this);

    public override string ToString() => Value?.ToString() ?? string.Empty;
}
