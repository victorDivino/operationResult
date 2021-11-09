﻿namespace OperationResult;

public struct Result<T>
{
    public T? Value { get; }
    public Exception? Exception { get; }
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(Value))]
    [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(false, nameof(Exception))]
#endif
    public bool IsSuccess { get; }

    public Result(T value)
    {
        IsSuccess = true;
        Exception = null;
        Value = value;
    }

    public Result(Exception exception)
    {
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        IsSuccess = false;
        Value = default;
    }

    public static implicit operator Result<T>(T value)
        => new(value);

    public static implicit operator Result<T>(Exception exception)
        => new(exception);

    public Result<TEnd> ChangeInAnotherResult<TEnd>(Func<T, TEnd> converter)
        => IsSuccess
            ? new Result<TEnd>(converter(Value))
            : new Result<TEnd>(Exception);

    public Result ChangeInNoResult()
        => IsSuccess
            ? new Result(true)
            : new Result(Exception);

    public void Deconstruct(out bool success, out T? value)
        => (success, value) = (IsSuccess, Value);

    public void Deconstruct(out bool success, out T? value, out Exception? exception)
        => (success, value, exception) = (IsSuccess, Value, Exception);

    public static implicit operator bool(Result<T> result)
        => result.IsSuccess;

    public static implicit operator Task<Result<T>>(Result<T> result)
        => result.AsTask;

    public Task<Result<T>> AsTask => Task.FromResult(this);
}

public struct Result
{
    public Exception? Exception { get; }
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(false, nameof(Exception))]
#endif
    public bool IsSuccess { get; }

    public Result(bool success)
    {
        IsSuccess = success;
        Exception = null;
    }

    public Result(Exception exception)
    {
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        IsSuccess = false;
    }

    public bool ErrorIs<TException>()
        where TException : Exception
        => Exception is TException;

    public static Result Success()
        => new(true);

    public static Result Error(Exception exception)
        => new(exception);

    public static Result<T> Success<T>(T value)
        => new(value);

    public static Result<T> Error<T>(Exception exception)
        => new(exception);

    public static implicit operator Result(Exception exception)
        => new(exception);

    public static implicit operator bool(Result result)
        => result.IsSuccess;

    public static implicit operator Task<Result>(Result result)
        => result.AsTask;

    public void Deconstruct(out bool success, out Exception? exception)
        => (success, exception) = (IsSuccess, Exception);

    public Task<Result> AsTask => Task.FromResult(this);
}
