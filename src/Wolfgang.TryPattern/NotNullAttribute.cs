// Polyfill for nullable-annotation attributes that ship in
// netstandard2.1 / .NET 5+ but are missing from the older TFMs we
// target (net462, netstandard2.0). Declared `internal` so consumers
// of this assembly never see them, and so they don't collide with the
// runtime-provided types on newer TFMs.
#if !NET5_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER

namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Specifies that an output is not <see langword="null"/> even if the
    /// corresponding type allows it. Used by the C# compiler's nullable
    /// flow analysis.
    /// </summary>
    [AttributeUsage
    (
        AttributeTargets.Field
        | AttributeTargets.Parameter
        | AttributeTargets.Property
        | AttributeTargets.ReturnValue,
        Inherited = false
    )]
    internal sealed class NotNullAttribute : Attribute
    {
    }
}

#endif
