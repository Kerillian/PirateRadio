using HarmonyLib;

namespace PirateRadio.Patches;

[HarmonyPatch(typeof(VehicleRadio))]
public class VehicleRadioPatch
{
	[HarmonyPostfix, HarmonyPatch(nameof(VehicleRadio.SetPlayingIO))]
	public static void SetPlayingIO(VehicleRadio __instance)
	{
		if (__instance.io != null)
		{
			__instance.io.description = Refs.radioStation.channels[__instance.currentChannelID].name;
		}
	}
}