namespace ImmutableSharp;

/// <summary>
/// Specifies that a method, property or field is immutable
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
public sealed class ImmutableAttribute : Attribute
{
}
