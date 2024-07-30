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
		if (Plugin.Channels.TryGetValue(__instance.name, out List<AudioClip> clips))
		{
			__instance.owner = owner;
			__instance.audioClips = clips;
			__instance.toPlay = new List<AudioClip>(__instance.audioClips);

			return false;
		}

		return true;
	}
}