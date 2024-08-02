using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace PirateRadio.Patches;

[HarmonyPatch(typeof(ItemHands))]
public class ItemHandsPatch
{
	[HarmonyPrefix, HarmonyPatch(nameof(ItemHands.LateUpdate))]
	public static bool LateUpdate(ItemHands __instance)
	{
		int delta = Mathf.RoundToInt(UnityInput.Current.mouseScrollDelta.y);
		
		if (delta != 0 && __instance.player.interaction?.currentSelectedIO?.interaction is IOInteractVehicleRadio interact && interact.radio.isOn)
		{
			Utility.ChangeChannel(interact.radio, interact.radio.currentChannelID + -delta);
			interact.radio.io.ShowInteractionPanel(true);
	
			return false;
		}

		return true;
	}
}