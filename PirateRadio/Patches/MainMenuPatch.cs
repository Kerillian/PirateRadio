using HarmonyLib;

namespace PirateRadio.Patches;

[HarmonyPatch(typeof(MainMenu))]
public class MainMenuPatch
{
	[HarmonyPostfix, HarmonyPatch(nameof(MainMenu.OnActivated))]
	public static void OnActivated()
	{
		if (!Plugin.StartupMessage.Value)
		{
			return;
		}
		
		string message = string.Join("\n",
			$"To quickly open the channels folder press {Plugin.OpenFolderKey.Value}.",
			$"After adding custom music you have to reload the channels with {Plugin.ReloadKey.Value}.",
			"",
			"Radio controls have also changed, Interact now just toggles the radio on and off.",
			"To change channels simply use your scroll wheel while looking at the radio.",
			$"You can also use {Plugin.PreviousKey.Value} and {Plugin.NextKey.Value} to change channels."
		);
		
		Manager.ui.ShowPopup("Hello from Pirate Radio!", message, PopupMessage.PopupButtonsType.OK);
		Plugin.StartupMessage.Value = false;
	}
}