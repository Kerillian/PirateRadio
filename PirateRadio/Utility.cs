using System;
using System.IO;
using UnityEngine;

namespace PirateRadio;

public class Utility
{
	public static AudioType AudioTypeFromString(string ext)
	{
		return ext switch
		{
			"aif" => AudioType.AIFF,
			"aiff" => AudioType.AIFF,
			"it" => AudioType.IT,
			"mod" => AudioType.MOD,
			"mp2" => AudioType.MPEG,
			"mp3" => AudioType.MPEG,
			"ogg" => AudioType.OGGVORBIS,
			"s3m" => AudioType.S3M,
			"wav" => AudioType.WAV,
			"xm" => AudioType.XM,
			"flac" => AudioType.UNKNOWN,
			_ => AudioType.UNKNOWN
		};
	}
	
	public static AudioType AudioTypeFromExtension(string file)
	{
		return AudioTypeFromString(Path.GetExtension(file).ToLowerInvariant().Substring(1));
	}

	// https://stackoverflow.com/a/9545731
	public static string KnuthHash(string str)
	{
		ulong hash = 3074457345618258791ul;

		for (int i = 0; i < str.Length; i++)
		{
			hash += str[i];
			hash *= 3074457345618258799ul;
		}

		return hash.ToString("X");
	}

	public static void ShowFolder(string path)
	{
		path = Path.GetFullPath(path);
		
		if (!Directory.Exists(path))
		{
			return;
		}

		string[] platform = Application.platform switch
		{
			RuntimePlatform.WindowsPlayer => ["explorer.exe", "/root,\"{0}\""],
			RuntimePlatform.LinuxPlayer => ["xdg-open", "{0}"],
			RuntimePlatform.OSXPlayer => ["open", "{0}"],
			_ => Array.Empty<string>()
		};
		
		try
		{
			if (platform.Length > 0)
			{
				System.Diagnostics.Process.Start(platform[0], string.Format(platform[1], path));
			}
		}
		catch (Exception e)
		{
			Console.Error.WriteLine(e.ToString());
		}
	}
}