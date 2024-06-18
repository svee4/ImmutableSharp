using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;

namespace ImmutableSharp.Analyzers;

partial class ImmutableMemberAnalyzer
{
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => Diagnostics.SupportedDiagnostics;

	public static class Diagnostics
	{
		private static DiagnosticDescriptor CreateDiagnosticDescriptor(string id, string title, string messageFormat, string description) =>
			new(
				id: id,
				title: title,
				messageFormat: messageFormat,
				description: description,
				category: "ImmutableSharp",
				defaultSeverity: DiagnosticSeverity.Error,
				isEnabledByDefault: true
		);

		public static ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [
			ImmutableFieldIsNotReadOnly,
			ImmutablePropertyHasSetter,
			ImmutableMethodSetsField,
			ImmutableMethodSetsProperty,
			ImmutableMethodCallsNonImmutableMethod
		];

		public static readonly DiagnosticDescriptor ImmutableFieldIsNotReadOnly =
			CreateDiagnosticDescriptor(
				id: $"{Utilities.Diagnostics.ImmutableFieldIsNotReadOnly}",
				title: "Immutable field must be readonly",
				messageFormat: "Immutable field {0} must be readonly",
				description: "A field annotated with ImmutableAttribute must have the readonly modifier."
		);


		public static readonly DiagnosticDescriptor ImmutablePropertyHasSetter =
			CreateDiagnosticDescriptor(
				id: $"{Utilities.Diagnostics.ImmutablePropertyHasSetter}",
				title: "Immutable property must not have a setter",
				messageFormat: "Immutable property {0} must not have a setter",
				description: "A property annotated with ImmutableAttribute must not have a setter."
		);

		public static readonly DiagnosticDescriptor ImmutableMethodSetsField =
			CreateDiagnosticDescriptor(
				id: $"{Utilities.Diagnostics.ImmutableMethodSetsField}",
				title: "Immutable method must not set the value of a field",
				messageFormat: "Immutable method {0} must not set the value of a field",
				description: "A method annotated with ImmutableAttribute must not set the value of a field."
		);

		public static readonly DiagnosticDescriptor ImmutableMethodSetsProperty =
			CreateDiagnosticDescriptor(
				id: $"{Utilities.Diagnostics.ImmutableMethodSetsProperty}",
				title: "Immutable method must not set the value of a property",
				messageFormat: "Immutable method {0} must not set the value of a property",
				description: "A method annotated with ImmutableAttribute must not set the value of a property."
		);

		public static readonly DiagnosticDescriptor ImmutableMethodCallsNonImmutableMethod =
			CreateDiagnosticDescriptor(
				id: $"{Utilities.Diagnostics.ImmutableMethodCallsNonImmutableMethod}",
				title: "Immutable method must not call a non-immutable method",
				messageFormat: "Immutable method {0} must not call non-immutable method {1}",
				description: "A method annotated with ImmutableAttribute must not call a method that is not annotated with ImmutableAttribute."
		);
	}

}
