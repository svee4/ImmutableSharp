using ImmutableSharp.Analyzers;

namespace ImmutableSharp.Tests.AnalyzerTests;

partial class Tests
{
	[Fact]
	public async Task ImmutableField_NotReadOnly_Error() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ImmutableMemberAnalyzer>(
			$$"""
			using System;
			using ImmutableSharp;

			class C
			{
				[Immutable]
				private int {|{{ImmutableMemberAnalyzer.Diagnostics.ImmutableFieldIsNotReadOnly.Id}}:_field|};
			}
			"""
		).RunAsync();
}
