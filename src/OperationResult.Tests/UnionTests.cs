namespace OperationResult.Tests;

public class UnionTests
{
    private static Result<int> Parse(string text)
    {
        if (int.TryParse(text, out var number))
            return number;
        return new FormatException($"'{text}' is not a number");
    }

    private static Result Save(bool succeed)
        => succeed ? Result.Success() : new InvalidOperationException("boom");

    private static string Describe(Result<int> result) => result switch
    {
        int value => $"value {value}",
        FormatException => "format",
        Exception error => $"error {error.Message}",
        null => "empty",
    };

    private static string Describe(Result result) => result switch
    {
        Ok => "ok",
        Exception error => error.Message,
        null => "empty",
    };

    [Fact]
    public void Implicit_conversion_from_value_creates_success()
    {
        var result = Parse("42");

        result.IsSuccess.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Value.Should().Be(42);
        Describe(result).Should().Be("value 42");
    }

    [Fact]
    public void Implicit_conversion_from_exception_creates_error()
    {
        var result = Parse("x");

        result.IsSuccess.Should().BeFalse();
        result.Exception.Should().BeOfType<FormatException>();
        result.Value.Should().BeSameAs(result.Exception);
        Describe(result).Should().Be("format");
    }

    [Fact]
    public void Non_generic_result_switches_over_ok_and_exception()
    {
        Describe(Save(true)).Should().Be("ok");
        Describe(Save(false)).Should().Be("boom");
        Save(true).Value.Should().BeSameAs(Ok.Instance);
    }

    [Fact]
    public void Default_result_is_empty_and_not_success()
    {
        Result<int> generic = default;
        Result plain = default;

        generic.IsSuccess.Should().BeFalse();
        generic.Exception.Should().BeNull();
        generic.Value.Should().BeNull();
        Describe(generic).Should().Be("empty");

        plain.IsSuccess.Should().BeFalse();
        plain.Exception.Should().BeNull();
        Describe(plain).Should().Be("empty");
    }

    [Fact]
    public void TryGetValue_returns_value_only_on_success()
    {
        Parse("7").TryGetValue(out var value).Should().BeTrue();
        value.Should().Be(7);

        Parse("x").TryGetValue(out _).Should().BeFalse();
        Parse("x").GetValueOrDefault(-1).Should().Be(-1);
    }

    [Fact]
    public void Success_with_null_value_throws()
    {
        Action factory = () => Result.Success<string?>(null);
        Action conversion = () => { Result<string?> _ = (string?)null; };

        factory.Should().Throw<ArgumentNullException>();
        conversion.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Implicit_conversion_from_null_exception_throws()
    {
        Action act = () => { Result _ = (Exception)null!; };

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ErrorIs_checks_exception_type()
    {
        Parse("x").ErrorIs<FormatException>().Should().BeTrue();
        Parse("x").ErrorIs<InvalidOperationException>().Should().BeFalse();
        Save(false).ErrorIs<InvalidOperationException>().Should().BeTrue();
        Save(true).ErrorIs<Exception>().Should().BeFalse();
    }

    [Fact]
    public void Deconstruct_returns_components()
    {
        var (success, value, exception) = Parse("3");
        success.Should().BeTrue();
        value.Should().Be(3);
        exception.Should().BeNull();

        var (ok, error) = Save(false);
        ok.Should().BeFalse();
        error.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void Bool_conversion_reflects_success()
    {
        bool ok = Parse("1");
        bool failed = Parse("x");

        ok.Should().BeTrue();
        failed.Should().BeFalse();
    }

    [Fact]
    public async Task AsTask_wraps_result()
    {
        var result = await Parse("5").AsTask;

        result.GetValueOrDefault().Should().Be(5);
    }

    [Fact]
    public void ChangeInAnotherResult_maps_value_or_keeps_error()
    {
        var mapped = Parse("2").ChangeInAnotherResult(n => Result.Success(n * 10));
        mapped.IsSuccess.Should().BeTrue();
        mapped.GetValueOrDefault().GetValueOrDefault().Should().Be(20);

        var failed = Parse("x").ChangeInAnotherResult(n => Result.Success(n * 10));
        failed.Exception.Should().BeOfType<FormatException>();
    }

    [Fact]
    public void ChangeInNoResult_drops_value()
    {
        Parse("2").ChangeInNoResult().IsSuccess.Should().BeTrue();
        Parse("x").ChangeInNoResult().Exception.Should().BeOfType<FormatException>();
    }

    [Fact]
    public void Type_patterns_look_inside_the_union()
    {
        Result<string> result = "hello";

        (result is string { Length: 5 }).Should().BeTrue();
        (result is Exception).Should().BeFalse();
        (result is not null).Should().BeTrue();
    }
}
