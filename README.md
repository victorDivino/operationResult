# Operation Result

[![Build Status](https://dev.azure.com/victordoamordivino/OperationResult/_apis/build/status/victorDivino.OperationResult?branchName=master)](https://dev.azure.com/victordoamordivino/OperationResult/_build/latest?definitionId=1&branchName=master) [![NuGet](https://img.shields.io/badge/Nuget-v3.0.0-blue)](https://www.nuget.org/packages/Divino.OperationResult) [![License: MIT](https://img.shields.io/github/license/victorDivino/operationResult)](https://github.com/victorDivino/operationResult/blob/master/LICENSE)

This a simple C# class libary to handle error of an operation.

## When to Use it?

Operation Result can be used for error handling without throwing costly `Exceptions`.

## Operation Result Pattern

The purpose of the Operation Result design pattern is to give an operation (a method) the possibility to return a complex result (an object), allowing the consumer to:

- Access the result of an operation; in case there is one.
- Access the success indicator of an operation.
- Access the cause of the failure in case the operation was not successful.

## OperationResult Namespace

Represents the result of an operation. The result must be immutable and its properties must be read-only.

```csharp
public struct Result
```

```csharp
public struct Result<T>
```

### Properties

Access the result of the operation, in case there is one.

```csharp
 public T Value { get; set; }
```

Access the success indicator of the operation.

```csharp
public bool IsSuccess { get; }
```

Access the cause of the failure in case the operation was not successful.

```csharp
public Exception Exception { get; }
```

### Methods

```csharp
public static Result Success()
    => new Result(true);
```

```csharp
public static Result<T> Success<T>(T value)
    => new Result<T>(value);
```

```csharp
public static Result Error(Exception exception)
    => new Result(exception);
```

```csharp
public static Result<T> Error<T>(Exception exception)
    => new Result<T>(exception);
```

## Implicit operators

```csharp
public static implicit operator Result<T>(T value)
    => new Result<T>(value);

public static implicit operator Result<T>(Exception exception)
    => new Result<T>(exception);
```

The following example demonstrates how to use it:

```csharp
Result<int> IncrementOne(int value)
{
    if (value > 9)
    {
        return new ArgumentOutOfRangeException(nameof(value), "Value cannot be greater than nine.");
    }
    return value++;
}
```

## Deconstructing a Result

```csharp
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
```

The following example demonstrates how to use it:

```csharp
public static void Main()
{
    var (isSuccess, value, exception) = IncrementOne(2);

    // Do something with the data.
}
```
