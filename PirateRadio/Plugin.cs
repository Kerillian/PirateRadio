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

[BepInPlugin("kerillian.pirate.radio", "Pirate Radio", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
	public static ConfigEntry<KeyCode> SkipKey;
	public static ConfigEntry<KeyCode> ReloadKey;
	public static ConfigEntry<KeyCode> OpenFolderKey;

	private new static ManualLogSource Logger;
	private static Harmony harmony;

	private static readonly string SongFolder = Path.Combine(Application.streamingAssetsPath, "PirateRadio");
	public static readonly List<AudioClip> Music = new List<AudioClip>();
	public static RadioChannel Channel = null;
	private static bool loading = false;
	private static bool wasRunning = false;

	private void Awake()
	{
		SkipKey = Config.Bind("Keybinds", "Skip", KeyCode.F1, "Skips the current track on channel.");
		ReloadKey = Config.Bind("Keybinds", "Reload", KeyCode.F2, "Reloads all custom tracks.");
		OpenFolderKey = Config.Bind("Keybinds", "Open", KeyCode.F3, "Opens the custom music folder.");
		
		harmony = new Harmony(Info.Metadata.GUID);
		harmony.PatchAll(GetType().Assembly);
		
		Logger = base.Logger;
		Logger.LogInfo($"Plugin {Info.Metadata.GUID} is loaded!");
		
		Directory.CreateDirectory(SongFolder);
		StartCoroutine(ReloadMusic());
	}

	private void Update()
	{
		if (loading)
		{
			return;
		}

		if (UnityInput.Current.GetKeyDown(SkipKey.Value))
		{
			if (Refs.garage != null)
			{
				VehicleRadio radio = Refs.garage.activeVehicle.io.radio;

				if (radio.isOn)
				{
					Refs.radioStation.channels[radio.currentChannelID].PlayNextClip();
				}
			}
		}

		if (UnityInput.Current.GetKeyDown(ReloadKey.Value))
		{
			StartCoroutine(ReloadMusic());
		}
		
		if (UnityInput.Current.GetKeyDown(OpenFolderKey.Value))
		{
			Utility.ShowFolder(SongFolder);
		}
	}

	public IEnumerator ReloadMusic()
	{
		if (Channel is { playCoroutine: not null })
		{
			if (Refs.garage != null)
			{
				Refs.garage.activeVehicle.io.radio.TurnOff();
			}
			
			wasRunning = true;
			Channel.Enable(false);
		}
		
		loading = true;

		foreach (AudioClip clip in Music)
		{
			clip.UnloadAudioData();
		}

		Music.Clear();

		yield return StartCoroutine(ScanFolder());
		loading = false;

		if (wasRunning)
		{
			wasRunning = false;
			Channel.Enable(true);
			Channel.PlayNextClip();
		}
	}

	public IEnumerator ScanFolder(string path = "")
	{
		string folder = path.Length == 0 ? SongFolder : path;
		
		foreach (string dir in Directory.GetDirectories(folder))
		{
			StartCoroutine(ScanFolder(dir));
		}

		foreach (string file in Directory.GetFiles(folder))
		{
			StartCoroutine(LoadFile(file));
		}

		yield return null;
	}

	public IEnumerator LoadFile(string file)
	{
		string fileName = Path.GetFileName(file);
		StartCoroutine(LoadAudioFromFile(file, Utility.KnuthHash(fileName), Utility.AudioTypeFromExtension(file)));
		
		yield return null;
	}

	public IEnumerator LoadAudioFromFile(string file, string name, AudioType type)
	{
		file = UnityWebRequest.EscapeURL(file);

		using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + file, type);
		yield return www.SendWebRequest();

		if (www.result == UnityWebRequest.Result.ConnectionError)
		{
			Logger.LogError(www.error);
		}
		else
		{
			DownloadHandlerAudioClip handler = (DownloadHandlerAudioClip)www.downloadHandler;
			handler.streamAudio = true;
			handler.audioClip.name = name;
			
			Music.Add(handler.audioClip);
				
			Logger.LogInfo($"Loaded {name}");
		}
	}
}