using FluentAssertions;
using System;
using Xunit;

namespace OperationResult.Tests;

public class OperationResultGenericTests
{
    [Fact]
    public void Result_Success_Should_true()
    {
        //Arrange
        var result = Result.Success(1);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(1);
        result.Exception.Should().BeNull();
    }

    [Fact]
    public void Result_Error_Should_False()
    {
        //Arrange
        var result = Result.Error<string>(new Exception("Error"));

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Exception.Should().BeEquivalentTo(new Exception("Error"));
    }

    [Fact]
    public void Result_Error_Should_Throw_Exception_When_Argument_Is_Null()
    {
        //Arrange
        Action act = () => Result.Error<string>(null!);

        //Assert
        act.Should().Throw<ArgumentNullException>().Where(e => e.Message.Contains("exception"));
    }

    [Fact]
    public void ResultT_Error_Should_Throw_Exception_When_Argument_Is_Null()
    {
        //Arrange
        Action act = () => Result.Error<string>(null!);

        //Assert
        act.Should().Throw<ArgumentNullException>().Where(e => e.Message.Contains("exception"));
    }
}
