#if !NET11_0_OR_GREATER
// C# 15 union types need these runtime types, which ship in .NET 11.
// Older target frameworks get internal copies so the compiler can lower `union` declarations.
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    internal sealed class UnionAttribute : Attribute;

    internal interface IUnion
    {
        object? Value { get; }
    }
}
#endif
