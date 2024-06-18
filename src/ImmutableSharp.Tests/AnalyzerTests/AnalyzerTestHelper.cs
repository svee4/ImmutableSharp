using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace ImmutableSharp.Tests.AnalyzerTests;

public static class AnalyzerTestHelpers
{
	public static CSharpAnalyzerTest<TAnalyzer, DefaultVerifier> CreateAnalyzerTest<TAnalyzer>(string source)
		where TAnalyzer : DiagnosticAnalyzer, new()
	{
		var csTest = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
		{
			TestState =
			{
				Sources = { source },
				ReferenceAssemblies = new ReferenceAssemblies(
					"net8.0",
					new PackageIdentity(
						"Microsoft.NETCore.App.Ref",
						"8.0.0"),
					Path.Combine("ref", "net8.0")),
			},
		};

		csTest.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile("./ImmutableSharp.dll"));

		return csTest;
	}
}
