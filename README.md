# Operation Result

[![Build Status](https://dev.azure.com/victordoamordivino/OperationResult/_apis/build/status/victorDivino.OperationResult?branchName=master)](https://dev.azure.com/victordoamordivino/OperationResult/_build/latest?definitionId=1&branchName=master) [![NuGet](https://img.shields.io/badge/Nuget-v5.0.0-blue)](https://www.nuget.org/packages/Divino.OperationResult) [![License: MIT](https://img.shields.io/github/license/victorDivino/operationResult)](https://github.com/victorDivino/operationResult/blob/master/LICENSE)

This a simple C# class libary to handle error of an operation.

## When to Use it?

Operation Result can be used for error handling without throwing costly `Exceptions`.

## Operation Result Pattern

The purpose of the Operation Result design pattern is to give an operation (a method) the possibility to return a complex result (an object), allowing the consumer to:

- Access the result of an operation; in case there is one.
- Access the success indicator of an operation.
- Access the cause of the failure in case the operation was not successful.

## Requirements

- Target frameworks: `net8.0`, `net10.0`, `net11.0`.
- Any C# version can use the properties, factories and implicit conversions.
- Pattern matching on the union cases needs **C# 15**. That is the default on `net11.0`; on `net8.0`/`net10.0`, add `<LangVersion>15</LangVersion>` (requires the .NET 11 SDK).

## OperationResult Namespace

Since 5.0, both results are [C# 15 union types](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/union). A result holds exactly one of its cases:

```csharp
public union Result(Ok, Exception);
public union Result<T>(T, Exception);
```

`Ok` is the success marker of the non-generic `Result` (`Ok.Instance`).

### Properties

Access the success indicator of the operation.

```csharp
public bool IsSuccess { get; }
```

Access the cause of the failure in case the operation was not successful.

```csharp
public Exception? Exception { get; }
```

Access the union contents (`T`/`Ok`, an `Exception`, or `null` for `default`).

```csharp
public object? Value { get; }
```

### Methods

```csharp
Result.Success();                  // Result
Result.Error(exception);           // Result
Result.Success<T>(value);          // Result<T>
Result.Error<T>(exception);        // Result<T>

result.TryGetValue(out T value);   // Result<T>: typed access to the value
result.GetValueOrDefault();        // Result<T>
result.GetValueOrDefault(fallback);
result.ErrorIs<TException>();
```

Success values cannot be `null`, and `Error` doesn't accept a `null` exception. Both throw `ArgumentNullException`.

## Implicit conversions

Return the value or the exception directly:

```csharp
Result<int> IncrementOne(int value)
{
    if (value > 9)
    {
        return new ArgumentOutOfRangeException(nameof(value), "Value cannot be greater than nine.");
    }
    return value + 1;
}
```

## Pattern matching (C# 15)

Type patterns look inside the union, and the compiler checks that every case is handled:

```csharp
string message = IncrementOne(2) switch
{
    int value => $"Got {value}",
    ArgumentOutOfRangeException => "Too big",
    Exception error => error.Message,
    null => "No result",   // default(Result<int>)
};
```

Prefer a `null` arm over `_`, so that the compiler can still warn you about unhandled cases.

## Deconstructing a Result

```csharp
public static void Main()
{
    var (isSuccess, value, exception) = IncrementOne(2);

    // Do something with the data.
}
```

## Migrating from 4.x

5.0 is a breaking release: `Result<T>.Value` is now the `object?` union contents (use `TryGetValue`/`GetValueOrDefault`), the public constructors were replaced by factories, and `null` success values are rejected.

The package ships an agent skill, **`operationresult-migration`**, which walks an AI coding agent through the migration. After restore, it is at `~/.nuget/packages/divino.operationresult/5.0.0/skills/operationresult-migration/SKILL.md` (see [Delivering skills in NuGet packages](https://devblogs.microsoft.com/dotnet/delivering-skills-in-nuget-packages/)). You can also read it in this repository: [skills/operationresult-migration/SKILL.md](skills/operationresult-migration/SKILL.md).
