using System.Runtime.CompilerServices;
using VerifyTests;

namespace ImmutableSharp.Tests;

public static class ModuleInitializer
{
	[ModuleInitializer]
	public static void Init()
	{
		VerifierSettings.AutoVerify(includeBuildServer: false);
		VerifySourceGenerators.Initialize();
	}
}
