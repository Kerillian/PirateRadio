<p align="center">
 <img src="./.media/PirateRadio.png" height="500px" alt="">
</p>

# Pirate Radio
This is a mod for [Contraband Police](https://store.steampowered.com/app/756800/Contraband_Police/). It adds a third radio station that plays custom audio files.

> [!IMPORTANT]
> This mod, "Pirate Radio," is purely a fictional and creative addition to enhance the gaming experience. It is themed around historical pirate radio broadcasts and does not promote, condone, or encourage any form of illegal activity or media piracy. The mod is intended solely for entertainment purposes within the context of the game.

# Install
1. Download [BepInEx 5.4](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.2) ([x64](https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.2/BepInEx_win_x64_5.4.23.2.zip) | [x32](https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.2/BepInEx_win_x86_5.4.23.2.zip))
2. Extract the archive into the root directory of Contraband Police
3. Download Pirate Radio ([Releases](https://github.com/Kerillian/PirateRadio/releases))
4. Place `PirateRadio.dll` in `BepInEx/plugins`
5. Place audio files in `Contraband Police\ContrabandPolice_Data\StreamingAssets\PirateRadio`
6. Profit

# Supported files
All audio formats Unity supports.
- mp3
- mp2
- ogg
- wav
- flac
- Tracker files
  - aif
  - aiff
  - it
  - mod
  - s3m
  - xm

# Development
Due to requiring libraries from the game, you have to setup a MSBuild property called `ConPolPath` that points to the games install location.
If you don't know how to add a global property I've written a few examples below for different tools.

> [!NOTE]
> Example of a valid path: `C:\Program Files (x86)\Steam\steamapps\common\Contraband Police`
> - The path should be absolute
> - No trailing slashes

### Rider
Settings **>** Build, Execution, Deployment **>** Toolset and Build **>** MSBuild global properties

### Dotnet CLI
Add the argument `-p:ConPolPath="Absolute/Path/Here"`

### Visual Studio
Visual Studio stores editor specific values in `.csproj.user`. So create or edit `PirateRadio.csproj.user` and add the xml tag `ConPolPath`


```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
  <PropertyGroup>
    <ConPolPath>Absolute/Path/Here</ConPolPath>
  </PropertyGroup>
</Project>
```

# Thanks
- Friends & Family. Love you.
- [BepInEx](https://github.com/BepInEx/BepInEx)
- [Kade](https://github.com/Kade-github)
  - [Their mod](https://github.com/Kade-github/BombRushRadio) for Bomb Rush was a massive help in understanding how to load files from StreamingAssets.