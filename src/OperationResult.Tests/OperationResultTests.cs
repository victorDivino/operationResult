using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OperationResult.Tests
{
    [TestClass]
    public class OperationResultTests
    {
        [TestMethod]
        public void Result_Success_Should_true()
        {
            //Arrange
            var result = Result.Success();

            //Assert
            result.IsSuccess.Should().BeTrue();
        }

        [TestMethod]
        public void Result_Error_Should_False()
        {
            //Arrange
            var result = Result.Error(new Exception());

            //Assert
            result.IsSuccess.Should().BeFalse();
        }
    }
}
