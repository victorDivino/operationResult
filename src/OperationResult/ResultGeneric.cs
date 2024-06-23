using System.Diagnostics.CodeAnalysis;

namespace OperationResult;

public readonly struct Result<T> : IResultValue<T, Result<T>>
{
    public T? Value { get; }
    public Exception? Exception { get; }
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Exception))]
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
        where TEnd : IResult<TEnd>
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

    static Result<T> IResultValue<T, Result<T>>.Success(T value) => new(value);

    static Result<T> IResult<Result<T>>.Success() => throw new NotImplementedException();

    static Result<T> IResult<Result<T>>.Error(Exception error) => new(error);

    public static implicit operator bool(Result<T> result)
        => result.IsSuccess;

    public static implicit operator Task<Result<T>>(Result<T> result)
        => result.AsTask;

    public Task<Result<T>> AsTask => Task.FromResult(this);
}