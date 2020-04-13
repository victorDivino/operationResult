using System;
using System.Threading.Tasks;

namespace OperationResult
{
    public struct Result<T>
    {
        public T Value { get; set; }
        public Exception Exception { get; }
        public bool IsSuccess { get; }

        public Result(T value)
        {
            IsSuccess = true;
            Exception = null;
            Value = value;
        }

        public Result(Exception exception)
        {
            Exception = exception;
            IsSuccess = false;
            Value = default;
        }

        public static implicit operator Result<T>(T value)
            => new Result<T>(value);

        public static implicit operator Result<T>(Exception exception)
            => new Result<T>(exception);

        public Result<TEnd> ChangeInAnotherValue<TEnd>(Func<T, TEnd> converter)
            => IsSuccess
                ? new Result<TEnd>(converter(Value))
                : new Result<TEnd>(Exception);

        public Result ChangeInNoResult()
            => IsSuccess
                ? new Result(true)
                : new Result(Exception);

        public void Deconstruct(out bool success, out T value)
        {
            success = IsSuccess;
            value = Value;
        }

        public void Deconstruct(out bool success, out T value, out Exception exception)
        {
            success = IsSuccess;
            value = Value;
            exception = Exception;
        }

        public static implicit operator bool(Result<T> result)
            => result.IsSuccess;

        public Task<Result<T>> AsTask => Task.FromResult(this);
    }

    public struct Result
    {
        public Exception Exception { get; }
        public bool IsSuccess { get; }

        public Result(bool success)
        {
            IsSuccess = success;
            Exception = null;
        }

        public Result(Exception exception)
        {
            Exception = exception;
            IsSuccess = false;
        }

        public bool ErrorIs<TException>()
            where TException : Exception
            => Exception is TException;

        public static Result Success()
            => new Result(true);

        public static Result Error(Exception exception)
            => new Result(exception);

        public static Result<T> Success<T>(T value)
            => new Result<T>(value);

        public static Result<T> Error<T>(Exception exception)
            => new Result<T>(exception);

        public static implicit operator Result(Exception exception)
            => new Result(exception);

        public static implicit operator bool(Result result)
            => result.IsSuccess;

        public void Deconstruct(out bool isSuccess, out Exception exception)
        {
            isSuccess = IsSuccess;
            exception = Exception;
        }

        public Task<Result> AsTask => Task.FromResult(this);
    }
}
