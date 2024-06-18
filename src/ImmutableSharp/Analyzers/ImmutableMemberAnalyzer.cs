using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace ImmutableSharp.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed partial class ImmutableMemberAnalyzer : DiagnosticAnalyzer
{
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
		context.EnableConcurrentExecution();

		context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Field, SymbolKind.Property);
		context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
	}

	private static void AnalyzeSymbol(SymbolAnalysisContext context)
	{
		var token = context.CancellationToken;
		token.ThrowIfCancellationRequested();

		var symbol = context.Symbol;

		if (!symbol
				.GetAttributes()
				.Any(x => x.AttributeClass?.ToString() == "ImmutableSharp.ImmutableAttribute"))
		{
			return;
		}

		switch (symbol)
		{
			case IFieldSymbol field:
				AnalyzeFieldSymbol(context, field);
				break;
			case IPropertySymbol property:
				AnalyzePropertySymbol(context, property);
				break;
			default:
				throw new InvalidOperationException($"Invalid symbol {symbol.GetType()}");
		}
	}

	private static void AnalyzeFieldSymbol(SymbolAnalysisContext context, IFieldSymbol symbol)
	{
		if (!symbol.IsReadOnly)
		{
			ReportDiagnosticWithName(context, Diagnostics.ImmutableFieldIsNotReadOnly, symbol);
		}
	}

	private static void AnalyzePropertySymbol(SymbolAnalysisContext context, IPropertySymbol symbol)
	{
		if (symbol.SetMethod is { IsInitOnly: false })
		{
			ReportDiagnosticWithName(context, Diagnostics.ImmutablePropertyHasSetter, symbol);
		}
	}

	private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
	{
		var syntax = (MethodDeclarationSyntax)context.Node;
		var symbol = context.SemanticModel.GetDeclaredSymbol(syntax) ?? throw new InvalidOperationException("Could not get method symbol from syntax");

		if (!symbol.GetAttributes().Any(IsImmutableAttribute))
		{
			return;
		}

		foreach (var operation in context.SemanticModel.GetOperation(syntax).Descendants())
		{
			switch (operation)
			{
				case IAssignmentOperation assignmentOperation:
					AnalyzeAssignmentOperation(context, assignmentOperation, symbol);
					break;
				case IInvocationOperation invocationOperation:
					AnalyzeInvocationOperation(context, invocationOperation, symbol);
					break;
			}

		}


		static void AnalyzeAssignmentOperation(SyntaxNodeAnalysisContext context, IAssignmentOperation operation, ISymbol symbol)
		{
			var diagnostic = operation.Target switch
			{
				ILocalFunctionOperation or IDiscardOperation => null,
				IPropertyReferenceOperation => Diagnostics.ImmutableMethodSetsProperty,
				IFieldReferenceOperation => Diagnostics.ImmutableMethodSetsField,
				_ => throw new InvalidOperationException($"Unhandled assignment operation target type {operation.Target.GetType()}")
			};

			if (diagnostic is null)
			{
				return;
			}

			context.ReportDiagnostic(Diagnostic.Create(
				diagnostic,
				operation.Syntax.GetLocation(),
				symbol.Name
			));
		}

		static void AnalyzeInvocationOperation(SyntaxNodeAnalysisContext context, IInvocationOperation operation, ISymbol symbol)
		{

			if (operation.TargetMethod.GetAttributes().Any(IsImmutableAttribute))
			{
				return;
			}

			context.ReportDiagnostic(Diagnostic.Create(
				Diagnostics.ImmutableMethodCallsNonImmutableMethod,
				operation.Syntax.GetLocation(),
				symbol.Name,
				operation.TargetMethod.Name
			));
		}
	}

	/// <summary>
	/// Reports a diagnostic with the given descriptor and the location of the given symbol, 
	/// with the symbol's name as the one and only message argument
	/// </summary>
	/// <param name="context"></param>
	/// <param name="diagnosticDescriptor"></param>
	/// <param name="symbol"></param>
	private static void ReportDiagnosticWithName(SymbolAnalysisContext context, DiagnosticDescriptor diagnosticDescriptor, ISymbol symbol) =>
		context.ReportDiagnostic(Diagnostic.Create(
			diagnosticDescriptor,
			symbol.Locations.First(),
			symbol.Name
		));

	/// <summary>
	/// Reports a diagnostic with the given descriptor and the location of the given symbol, 
	/// with the symbol's name as the one and only message argument
	/// </summary>
	/// <param name="context"></param>
	/// <param name="diagnosticDescriptor"></param>
	/// <param name="symbol"></param>
	private static void ReportDiagnosticWithName(SyntaxNodeAnalysisContext context, DiagnosticDescriptor diagnosticDescriptor, ISymbol symbol) =>
		context.ReportDiagnostic(Diagnostic.Create(
			diagnosticDescriptor,
			symbol.Locations.First(),
			symbol.Name
		));

	private static bool IsImmutableAttribute(AttributeData attributeData) =>
		attributeData.AttributeClass?.ToString() == "ImmutableSharp.ImmutableAttribute";
}
