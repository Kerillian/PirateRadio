using System.Runtime.CompilerServices;
using HarmonyLib;

namespace PirateRadio.Patches;

[HarmonyPatch(typeof(IOInteractVehicleRadio))]
public class IOInteractVehicleRadioPatch
{
	[HarmonyPrefix, HarmonyPatch(nameof(IOInteractVehicleRadio.Interact))]
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool Interact(IOInteractVehicleRadio __instance, object caller)
	{
		AudioZone audioZone = Helpers.GetAudioZone(__instance.owner.transform);
		
		if (__instance.radio.isOn)
		{
			__instance.radio.TurnOff();
			__instance.radio.SetPlayingIO();
			__instance.radio.io.ShowInteractionPanel(true);
			Manager.audio.PlaySound3D("light_switch_off", __instance.owner.transform.position, audioZone, 0.7f, 0.5f);
		}
		else
		{
			__instance.radio.TurnOn();
			__instance.radio.SetPlayingIO();
			__instance.radio.io.ShowInteractionPanel(true);
			Manager.audio.PlaySound3D("light_switch_on", __instance.owner.transform.position, audioZone, 0.7f, 0.5f);
		}
		
		return false;
	}
}