using ImmutableSharp.Analyzers;

namespace ImmutableSharp.Tests.AnalyzerTests;

partial class Tests
{
	[Fact]
	public async Task ImmutableMethod_SetsField_Error() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ImmutableMemberAnalyzer>(
			$$"""
			using System;
			using ImmutableSharp;

			class C
			{
				private int _field;

				[Immutable]
				public void M()
				{
					{|{{ImmutableMemberAnalyzer.Diagnostics.ImmutableMethodSetsField.Id}}:_field = 1|};
				}
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ImmutableMethod_SetsProperty_Error() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ImmutableMemberAnalyzer>(
			$$"""
			using System;
			using ImmutableSharp;

			class C
			{
				public int Field { get; set; }
			
				[Immutable]
				public void M()
				{
					{|{{ImmutableMemberAnalyzer.Diagnostics.ImmutableMethodSetsProperty.Id}}:Field = 1|};
				}			
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ImmutableMethod_CallsNonImmutableMethod_Error() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ImmutableMemberAnalyzer>(
			$$"""
			using System;
			using ImmutableSharp;

			class C
			{
				[Immutable]
				public void M1()
				{
					{|{{ImmutableMemberAnalyzer.Diagnostics.ImmutableMethodCallsNonImmutableMethod.Id}}:M2()|};
				}

				public void M2() {}
			}
			"""
		).RunAsync();

	[Fact]
	public async Task ImmutableMethod_AssignsLocalToMethodGroup_NoError() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ImmutableMemberAnalyzer>(
			$$"""
				using System;
				using ImmutableSharp;

				class C
				{
					[Immutable]
					public void M1()
					{
						var x = M2;
					}

					public int M2() => 1;
				}
				"""
		).RunAsync();

	[Fact]
	public async Task ImmutableMethod_CallsBCL_Error() =>
		await AnalyzerTestHelpers.CreateAnalyzerTest<ImmutableMemberAnalyzer>(
			$$"""
				using System;
				using ImmutableSharp;

				class C
				{
					[Immutable]
					public void M()
					{
						_ = {|{{ImmutableMemberAnalyzer.Diagnostics.ImmutableMethodCallsNonImmutableMethod.Id}}:int.Parse("")|};
					}
				}
				"""
		).RunAsync();
}
