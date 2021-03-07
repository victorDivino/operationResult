using System;
using FluentAssertions;
using Xunit;

namespace OperationResult.Tests
{
    public class OperationResultTests
    {
        [Fact]
        public void Result_Success_Should_true()
        {
            //Arrange
            var result = Result.Success();

            //Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void ResultT_Success_Should_true()
        {
            //Arrange
            var result = Result.Success(2);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(2);
        }

        [Fact]
        public void Result_Error_Should_False()
        {
            //Arrange
            var result = Result.Error(new Exception());

            //Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void ResultT_Error_Should_False()
        {
            //Arrange
            var result = Result.Error<int>(new Exception());

            //Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void ChangeInAnotherResult_WhenResultIsSucess_ShouldCallFuncDelegate()
        {
            //Arrange
            var count = 0L;
            var result = Result.Success(0);

            //Act
            var changedResult = result.ChangeInAnotherResult<long>((t) =>
            {
                return ++count;
            });

            //Assert
            changedResult.Value.Should().BeOfType(typeof(long)).And.Be(1L);
            count.Should().Be(1L);
        }

        [Fact]
        public void ChangeInAnotherResult_WhenResultIsError_ShouldNotCallFuncDelegate()
        {
            //Arrange
            var count = 0L;
            var exception = new Exception("error");
            var result = Result.Error<int>(exception);

            //Act
            var changedResult = result.ChangeInAnotherResult<long>((t) =>
            {
                return ++count;
            });

            //Assert
            changedResult.Value.Should().Be(default);
            changedResult.Exception.Should().BeSameAs(exception);
            count.Should().Be(0L);
        }

        [Fact]
        public void ChangeInNoResult_WhenResultIsSuccess_ShouldReturnSuccessResult()
        {
            //Arrange                      
            var result = Result.Success(1);

            //Act
            var changedResult = result.ChangeInNoResult();

            //Assert
            changedResult.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void ChangeInNoResult_WhenResultIsError_ShouldReturnErrorResult()
        {
            //Arrange                      
            var exception = new Exception("error");
            var result = Result.Error<int>(exception);

            //Act
            var changedResult = result.ChangeInNoResult();

            //Assert
            changedResult.IsSuccess.Should().BeFalse();
            changedResult.Exception.Should().BeSameAs(exception);
        }
    }
}
