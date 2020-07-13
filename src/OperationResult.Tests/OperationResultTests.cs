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
        public void Result_Error_Should_False()
        {
            //Arrange
            var result = Result.Error(new Exception());

            //Assert
            result.IsSuccess.Should().BeFalse();
        }
    }
}
