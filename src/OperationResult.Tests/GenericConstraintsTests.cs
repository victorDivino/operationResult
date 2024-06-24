namespace OperationResult.Tests;
public class GenericConstraintsTests
{
    [Fact]
    public async Task CallToStaticErrorMethodFromGenericTypeDefinitionReturnsGenericResult()
    {
        //Arrange
        RequestWithResultValue request = new("data");
        var behavior = new ExceptionPipelineBehavior<RequestWithResultValue, Result<SimpleResponse>>();

        //Act
        var result = await behavior.Handle(request, Next);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<NotImplementedException>();

        static Task<Result<SimpleResponse>> Next(RequestWithResultValue request)
            => throw new NotImplementedException();
    }

    [Fact]
    public async Task CallToStaticErrorMethodFromGenericTypeDefinitionReturnsNonGenericResult()
    {
        //Arrange
        RequestWithNoResultValue request = new("data");
        var behavior = new ExceptionPipelineBehavior<RequestWithNoResultValue, Result>();

        //Act
        var result = await behavior.Handle(request, Next);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<NotImplementedException>();

        static Task<Result> Next(RequestWithNoResultValue request)
            => throw new NotImplementedException();
    }
}

public record RequestWithNoResultValue(string Value) : IRequest<Result>;
public record RequestWithResultValue(string Value) : IRequest<Result<SimpleResponse>>;
public record SimpleResponse(string Value);

public interface IRequest<TResponse>;

public interface IPipelineBehavior<TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request, Func<TRequest, Task<TResponse>> next);
}

public sealed class ExceptionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, Func<TRequest, Task<TResponse>> next)
    {
        try
        {
            return await next(request);
        }
        catch (Exception ex)
        {
            return TResponse.Error(ex);
        }
    }
}