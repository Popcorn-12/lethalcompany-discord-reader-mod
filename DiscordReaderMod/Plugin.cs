using System;
using System.Diagnostics.CodeAnalysis;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using static DiscordReaderMod.DiscordIngameProcess;

namespace DiscordReaderMod;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{

    public static Plugin Instance;
    private static Harmony _harmony;
    [NotNull] public static ManualLogSource Log { get; private set; }
    public static Configuration configuration;

    private async void Awake()
    {
        Instance = this;

        _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        _harmony.PatchAll(typeof(DiscordIngameProcess));
        Log = Logger;

        configuration = new(Config);

        await DiscordAPI.Main(Log);

        if (configuration.MappingUsers != null && configuration.MappingUsers.Length != 0)
        {
            // Check if string is not the default message
            if (!configuration.MappingUsers.Equals(configuration.defaultMappingUsers))
            {
                LoadUsernameFromConfig(configuration.MappingUsers);
            }
        }

        Logger.LogInfo("Started Discord Chat Plugin Successfully.");
    }

}
