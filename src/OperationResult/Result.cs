namespace OperationResult;

public struct Result : IResult
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
