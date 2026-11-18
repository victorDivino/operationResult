# Before/after examples (Divino.OperationResult 4.x → 5.x)

## Returning results (unchanged)

```csharp
// 4.x and 5.x: identical
Result<int> IncrementOne(int value)
{
    if (value > 9)
        return new ArgumentOutOfRangeException(nameof(value), "Value cannot be greater than nine.");
    return value + 1;
}

Result Delete(int id) => id > 0 ? Result.Success() : Result.Error(new ArgumentException("Invalid id"));
```

## Reading the value

```csharp
// 4.x
var result = repository.Find(id);
if (result.IsSuccess)
    return Ok(result.Value.Name);
return Problem(result.Exception.Message);

// 5.x: TryGetValue (any C# version)
var result = repository.Find(id);
if (result.TryGetValue(out var user))
    return Ok(user.Name);
return Problem(result.Exception!.Message);

// 5.x: exhaustive switch (C# 15)
return repository.Find(id) switch
{
    User user => Ok(user.Name),
    Exception error => Problem(error.Message),
    null => Problem("Empty result"),
};
```

## Constructors

```csharp
// 4.x
Result ok = new Result(true);
Result<Order> created = new Result<Order>(order);
Result failed = new Result(new TimeoutException());

// 5.x
Result ok = Result.Success();
Result<Order> created = order;                    // or Result.Success(order)
Result failed = new TimeoutException();           // or Result.Error(new TimeoutException())
```

## Null success values

```csharp
// 4.x: a success holding null
Result<Customer?> FindCustomer(string email) => db.Customers.FirstOrDefault(c => c.Email == email);

// 5.x: a null success throws ArgumentNullException, so make "not found" an explicit error
Result<Customer> FindCustomer(string email)
    => db.Customers.FirstOrDefault(c => c.Email == email) is { } customer
        ? customer
        : new KeyNotFoundException($"Customer '{email}' not found");
```

## Generic pipeline behaviors (unchanged)

```csharp
public sealed class ExceptionBehavior<TRequest, TResponse>
    where TResponse : IResult<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, Func<TRequest, Task<TResponse>> next)
    {
        try { return await next(request); }
        catch (Exception ex) { return TResponse.Error(ex); }   // still works for Result and Result<T>
    }
}
```

## Deconstruction (unchanged)

```csharp
var (success, value, exception) = IncrementOne(2);   // value is T? (default on error)
var (ok, error) = Delete(1);
```
