using HarmonyLib;

namespace PirateRadio.Patches;

[HarmonyPatch(typeof(RadioStation))]
public class RadioStationPatch
{
	[HarmonyPrefix, HarmonyPatch(nameof(RadioStation.Init))]
	public static void Init(RadioStation __instance)
	{
		foreach (string channel in Plugin.Channels.Keys)
		{
			__instance.channels.Add(new RadioChannel
			{
				name = channel
			});
		}
	}
}