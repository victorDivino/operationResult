namespace OperationResult;

/// <summary>
/// The success case of a non-generic <see cref="Result"/>.
/// </summary>
public sealed class Ok
{
    /// <summary>The single <see cref="Ok"/> instance.</summary>
    public static Ok Instance { get; } = new();

    private Ok() { }

    public override string ToString() => nameof(Ok);
}
