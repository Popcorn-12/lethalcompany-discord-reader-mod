using System.Diagnostics.CodeAnalysis;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace DiscordReaderMod;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{

    public static Plugin Instance;
    private readonly Harmony _harmony = new(PluginInfo.PLUGIN_GUID);
    [NotNull] public static ManualLogSource Log { get; private set; }

    private async void Awake()
    {
        Instance ??= this;

        _harmony.PatchAll(typeof(Plugin));
        Log = Logger;

        await DiscordAPI.Main(Log);

        Logger.LogInfo("Started Discord Chat Plugin Successfully.");
    }

}
