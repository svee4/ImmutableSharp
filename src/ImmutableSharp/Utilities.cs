namespace ImmutableSharp;

internal static class Utilities
{
	public const string DiagnosticPrefix = "IMS";

	public static class Diagnostics
	{
		public const string ImmutableFieldIsNotReadOnly = $"{DiagnosticPrefix}0001";
		public const string ImmutablePropertyHasSetter = $"{DiagnosticPrefix}0002";
		public const string ImmutableMethodSetsField = $"{DiagnosticPrefix}0003";
		public const string ImmutableMethodSetsProperty = $"{DiagnosticPrefix}0004";
		public const string ImmutableMethodCallsNonImmutableMethod = $"{DiagnosticPrefix}0005";
	}
}
