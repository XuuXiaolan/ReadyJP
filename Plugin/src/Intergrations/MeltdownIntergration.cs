using System.Runtime.CompilerServices;
using FacilityMeltdown.API;

namespace JohnPaularatus.Intergrations;

static class MeltdownIntegration {
	internal static bool Enabled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("me.loaforc.facilitymeltdown");

	
	// This extra function is needeed because trying to call meltdown functions at all without it would throw an exception otherwise
	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	internal static void TryCauseMeltdown() {
		if(Enabled) CauseMeltdown();
	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	static void CauseMeltdown() {
		MeltdownAPI.StartMeltdown(PluginInfo.PLUGIN_GUID);
	}
}