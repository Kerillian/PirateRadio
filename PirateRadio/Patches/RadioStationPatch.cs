using HarmonyLib;

namespace PirateRadio.Patches;

[HarmonyPatch(typeof(RadioStation))]
public class RadioStationPatch
{
	// Simply adds a new radio channel to the RadioStation
	[HarmonyPrefix, HarmonyPatch(nameof(RadioStation.Init))]
	public static void Init(RadioStation __instance)
	{
		__instance.channels.Add(new RadioChannel
		{
			name = "Pirate Radio"
		});
	}
}