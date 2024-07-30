using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace PirateRadio.Patches;

[HarmonyPatch(typeof(RadioChannel))]
public class RadioChannelPatch
{
	[HarmonyPrefix, HarmonyPatch(nameof(RadioChannel.Init))]
	public static bool Init(RadioChannel __instance, RadioStation owner)
	{
		if (__instance.name == "Pirate Radio")
		{
			Plugin.Channel = __instance;
			
			__instance.owner = owner;
			__instance.audioClips = Plugin.Music;
			__instance.toPlay = new List<AudioClip>(__instance.audioClips);

			return false;
		}

		return true;
	}
}