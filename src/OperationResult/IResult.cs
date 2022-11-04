namespace OperationResult;

public interface IResult
{
    Exception? Exception { get; }
    bool IsSuccess { get; }
}