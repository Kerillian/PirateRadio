using System.Collections;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;
using AudioClip = UnityEngine.AudioClip;

namespace PirateRadio;

[BepInPlugin("kerillian.pirate.radio", "Pirate Radio", "1.2.0")]
public class Plugin : BaseUnityPlugin
{
	public static ConfigEntry<bool> StartupMessage;
	public static ConfigEntry<KeyCode> NextKey;
	public static ConfigEntry<KeyCode> PreviousKey;
	public static ConfigEntry<KeyCode> ToggleKey;
	public static ConfigEntry<KeyCode> SkipKey;
	public static ConfigEntry<KeyCode> ReloadKey;
	public static ConfigEntry<KeyCode> OpenFolderKey;

	public new static ManualLogSource Logger;
	private static Harmony harmony;

	private static readonly string ChannelsFolder = Path.Combine(Paths.GameRootPath, "Channels");
	public static readonly Dictionary<string, List<AudioClip>> Channels = new Dictionary<string, List<AudioClip>>();
	
	private static bool loading = false;
	private static bool wasRunning = false;

	private void Awake()
	{
		harmony = new Harmony(Info.Metadata.GUID);
		Logger = base.Logger;

		StartupMessage = Config.Bind("General", "StartupMessage", true, "Used internally to only show the startup message once.");
		NextKey = Config.Bind("Keybinds", "NextChannel", KeyCode.RightArrow, "Change to the next channel.");
		PreviousKey = Config.Bind("Keybinds", "PreviousChannel", KeyCode.LeftArrow, "Change to the previous channel.");
		ToggleKey = Config.Bind("Keybinds", "Toggle", KeyCode.F5, "Toggles the radio on and off.");
		SkipKey = Config.Bind("Keybinds", "Skip", KeyCode.F6, "Skips track on the current channel.");
		ReloadKey = Config.Bind("Keybinds", "Reload", KeyCode.F7, "Reloads all custom tracks.");
		OpenFolderKey = Config.Bind("Keybinds", "Open", KeyCode.F8, "Opens the custom music folder.");
		
		harmony.PatchAll(GetType().Assembly);
		Logger.LogInfo($"Plugin {Info.Metadata.GUID} is loaded!");
		
		Directory.CreateDirectory(ChannelsFolder);
		StartCoroutine(ReloadMusic());
	}

	private void Update()
	{
		if (loading)
		{
			return;
		}

		IInputSystem input = UnityInput.Current;

		if (input.GetKeyDown(ToggleKey.Value))
		{
			if (Manager.players?.mainPlayer?.vehicle?.io?.radio is { isOn: true } radio)
			{
				if (radio.IsOn())
				{
					radio.TurnOff();
				}
				else
				{
					radio.TurnOn();
				}
			}
		}

		if (input.GetKeyDown(SkipKey.Value))
		{
			if (Refs.garage is not null)
			{
				VehicleRadio radio = Refs.garage.activeVehicle.io.radio;

				if (radio.isOn)
				{
					Refs.radioStation.channels[radio.currentChannelID].PlayNextClip();
				}
			}
		}

		if (input.GetKeyDown(NextKey.Value))
		{
			if (Manager.players?.mainPlayer?.vehicle?.io?.radio is { isOn: true } radio)
			{
				Utility.NextChannel(radio);
			}
		}
		
		if (input.GetKeyDown(PreviousKey.Value))
		{
			if (Manager.players?.mainPlayer?.vehicle?.io?.radio is { isOn: true } radio)
			{
				Utility.PreviousChannel(radio);
			}
		}

		if (input.GetKeyDown(ReloadKey.Value))
		{
			StartCoroutine(ReloadMusic());
		}
		
		if (input.GetKeyDown(OpenFolderKey.Value))
		{
			Utility.ShowFolder(ChannelsFolder);
		}
	}

	public IEnumerator ReloadMusic()
	{
		if (Refs.radioStation is { running: true })
		{
			if (Refs.garage is not null)
			{
				Refs.garage.activeVehicle.io.radio.TurnOff();
			}
			
			wasRunning = true;
			Refs.radioStation.EnableRadioStation(false);
		}
		
		loading = true;

		foreach (List<AudioClip> clips in Channels.Values)
		{
			foreach (AudioClip clip in clips)
			{
				clip.UnloadAudioData();
			}
			
			clips.Clear();
		}
		
		Channels.Clear();

		yield return StartCoroutine(ScanFolder());
		loading = false;

		if (wasRunning && Refs.radioStation is not null)
		{
			wasRunning = false;
			Refs.radioStation.Init();

			// Should fix the random audio stutter issue that happens when reloading the tracks.
			foreach (RadioChannel channel in Refs.radioStation.channels)
			{
				channel.PlayNextClip();
			}
		}
	}

	public IEnumerator ScanFolder()
	{
		foreach (string directory in Directory.GetDirectories(ChannelsFolder))
		{
			DirectoryInfo info = new DirectoryInfo(directory);

			if (!info.Exists)
			{
				continue;
			}
			
			FileInfo[] files = info.GetFiles();

			if (files.Length == 0)
			{
				continue;
			}
			
			Channels.Add(info.Name, new List<AudioClip>());
			
			foreach (FileInfo file in files)
			{
				yield return StartCoroutine(LoadFile(info.Name, file));
			}
		}

		yield return null;
	}

	public IEnumerator LoadFile(string channel, FileInfo file)
	{
		string songName = Utility.KnuthHash(file.Name);
		AudioType type = Utility.AudioTypeFromExtension(file.Name);
		string path = UnityWebRequest.EscapeURL(file.FullName);
		
		using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + path, type);
		yield return www.SendWebRequest();
		
		if (www.result == UnityWebRequest.Result.ConnectionError)
		{
			Logger.LogError(www.error);
		}
		else
		{
			DownloadHandlerAudioClip handler = (DownloadHandlerAudioClip)www.downloadHandler;
			handler.streamAudio = true;
			handler.audioClip.name = songName;
			
			Channels[channel].Add(handler.audioClip);
			
			Logger.LogInfo($"Loaded '{file.Name}' for channel '{channel}'");
		}
		
		yield return null;
	}
}