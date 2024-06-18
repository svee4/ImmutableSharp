using ImmutableSharp.Analyzers;

namespace ImmutableSharp.Tests.AnalyzerTests;

partial class Tests
{
	[Fact]
	public async Task ImmutableProperty_HasSetter_Error() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ImmutableMemberAnalyzer>(
			$$"""
			using System;
			using ImmutableSharp;

			class C
			{
				[Immutable]
				public int {|{{ImmutableMemberAnalyzer.Diagnostics.ImmutablePropertyHasSetter.Id}}:Prop|} { get; set; }
			}
			"""
		).RunAsync();
}
