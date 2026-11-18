---
name: operationresult-migration
description: Migrate C# code from Divino.OperationResult 4.x to 5.x, where Result and Result<T> became C# 15 union types. Use when upgrading the Divino.OperationResult package, when code reads result.Value as a typed value or constructs new Result(true) / new Result<T>(...), or when the build reports CS0029, CS0266, CS1061, CS1503, CS8121, CS8509 or CS8655 around Result/Result<T>.
version: 5.0.0
---

# Migrating Divino.OperationResult 4.x → 5.x

In 5.0, `Result` and `Result<T>` are C# 15 **union types**:

```csharp
public union Result(Ok, Exception);      // Ok is a singleton marker: Ok.Instance
public union Result<T>(T, Exception);
```

These stay the same: `IsSuccess`, `Exception`, `Result.Success()`, `Result.Success<T>(value)`,
`Result.Error(...)`, `Result.Error<T>(...)`, implicit conversions from `T`/`Exception`,
`implicit operator bool`, `AsTask`, `ErrorIs<TException>()`, both `Deconstruct` overloads,
`ChangeInAnotherResult`, `ChangeInNoResult`, and the `IResult<TSelf>` / `IResultValue<TIn, TSelf>` interfaces.

## 1. Upgrade the package and the language version

1. Set `<PackageReference Include="Divino.OperationResult" Version="5.0.0" />`.
2. The package targets `net8.0`, `net10.0` and `net11.0`. A project on `net6.0`/`net7.0` must move to `net8.0` or later, or stay on 4.x.
3. Code in the 4.x style (properties, factories, `return value;`) compiles with **any** C# version.
   **Pattern matching on the union** (`result switch { int v => ... }`) needs C# 15 in the consuming project:
   - `net11.0`: nothing to do, because C# 15 is the default.
   - `net8.0`/`net10.0`: add `<LangVersion>15</LangVersion>` (requires the .NET 11 SDK). Never use `latest`.

## 2. Find every usage

Search the solution before editing:

```bash
grep -rnE "\.Value\b|new Result(<[^>]+>)?\(|\.Success\(null|IResultValue|IResult<" --include=*.cs .
```

Then build (`dotnet build`) and fix each diagnostic using the table in section 3.

## 3. Breaking changes and fixes

| 4.x code | Symptom in 5.x | 5.x code |
| --- | --- | --- |
| `int n = result.Value;` on `Result<T>` | CS0266/CS0029: `Value` is now `object?`, the union contents | `int n = result.GetValueOrDefault();` or `if (result.TryGetValue(out var n)) { ... }` |
| `result.Value.Name` | CS1061 on `object` | `result.TryGetValue(out var v) ? v.Name : ...` or `result switch { User u => u.Name, ... }` |
| `result.Value ?? fallback` | The type is `object` | `result.GetValueOrDefault(fallback)` |
| `if (result.IsSuccess) Use(result.Value);` | `Value` no longer narrows to `T` | `if (result.TryGetValue(out var value)) Use(value);` |
| `new Result(true)` | CS1503: there is no `bool` constructor | `Result.Success()` |
| `new Result(false)` | CS1503 | `Result.Error(new SomeException(...))`. In 4.x, `false` without an exception was never a consistent state. |
| `new Result(ex)`, `new Result<T>(value)`, `new Result<T>(ex)` | Still compiles but **skips the null check** | `Result.Error(ex)`, `Result.Success(value)`, `Result.Error<T>(ex)`, or simply `return value;` / `return ex;` |
| `Result.Success<string?>(null)`, or `return (string?)null;` into `Result<string?>` | **Run-time** `ArgumentNullException`: a success value can't be null | Return an error (for example `new KeyNotFoundException(...)`), or use a value type that can say "nothing" |
| Generic code calling `TResponse.Success()` where `TResponse` is `Result<T>` | Throws `NotSupportedException` (it was `NotImplementedException`) | Call `TResponse.Success(value)` through `IResultValue<TIn, TSelf>` |
| `Result<Exception>`, `Result<object>` | Ambiguous: an exception would match both `T` and `Exception` | Use a dedicated type for `T` |
| `result.Value is SomeType x` | Still works (it tests the contents), but isn't idiomatic | `result is SomeType x`: type patterns look **inside** the union |

**No change in behavior:** `default(Result)` and `default(Result<T>)` are neither success nor error
(`IsSuccess == false`, `Exception == null`). In 5.x the union's `Value` is `null` in that state.

**Performance:** union cases are stored as `object?`, so a value-type `T` (for example `Result<int>`) is boxed once on creation.

## 4. Adopt the new patterns (optional, C# 15)

Replace `if/else` chains on `IsSuccess` with exhaustive switches. Type patterns look inside the union:

```csharp
string message = result switch
{
    User user => $"Hello {user.Name}",
    KeyNotFoundException => "No such user",
    Exception error => $"Failed: {error.Message}",
    null => "No result",            // default(Result<User>)
};

string status = Save() switch
{
    Ok => "saved",
    Exception error => error.Message,
    null => "not run",
};
```

Rules:
- Use a `null` arm, **not** `_`. A `_` arm hides future missing cases. Without a `null` arm the code compiles,
  but a `default` result throws `SwitchExpressionException` at run time.
- Put specific exception arms (`KeyNotFoundException`) **before** the general `Exception` arm. The reverse order gives CS8510.
- `result is Result<User>` gives CS8121, because a union isn't one of its own cases. Use `result is not null` to check
  for contents, or `result.IsSuccess` / `result is User` to check for success.
- `{ }` always matches a union struct, including `default`. Don't use it as a "has a value" check.

## 5. Verify

1. `dotnet build` with no new warnings. Watch for CS8509/CS8655 on switches over results.
2. Run the tests, especially code paths that returned `null` as a success value. Those now throw `ArgumentNullException`.
3. Search for leftover casts such as `(T)result.Value!` and replace them with `TryGetValue`.

See [references/examples.md](references/examples.md) for complete before/after examples.
