using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace PirateRadio.Patches;

[HarmonyPatch(typeof(VehicleRadio))]
public class VehicleRadioPatch
{
	[HarmonyPrefix, HarmonyPatch(nameof(VehicleRadio.SetPlayingIO))]
	public static bool SetPlayingIO(VehicleRadio __instance)
	{
		if (__instance.io != null)
		{
			if (__instance.isOn)
			{
				__instance.io.displayName = Manager.translator.Translate("#radio_tur_off");
			}
			else
			{
				__instance.io.displayName = Manager.translator.Translate("#radio_tur_on");
			}
			
			__instance.io.description = Refs.radioStation.channels[__instance.currentChannelID].name + "\n\nScroll wheel to change channels.";
		}
		
		return false;
	}
	
	/*
	 * This redirects the `this.ChangeChannel(0)` call from the VehicleRadio.TurnOff method.
	 * Did I really use a transpiler to stop a single call?
	 * Yes. Yes I did.
	 *
	 * If you're going to do something, do it right.
	 */
	[HarmonyTranspiler, HarmonyPatch(nameof(VehicleRadio.TurnOff))]
	public static IEnumerable<CodeInstruction> TurnOff(IEnumerable<CodeInstruction> instructions)
	{
		List<CodeInstruction> il = instructions.ToList();

		// Check if the method has changed. If it has, don't modify the list.
		if (il[19].opcode == OpCodes.Ldarg_0 && il[20].opcode == OpCodes.Ldc_I4_0 && il[21].opcode == OpCodes.Call)
		{
			// Removes
			// IL_0039: ldarg.0
			// IL_003A: ldc.i4.0
			// IL_003B: call      instance void VehicleRadio::ChangeChannel(int32)
			
			il.RemoveRange(19, 3);
		}
		
		return il;
	}
}