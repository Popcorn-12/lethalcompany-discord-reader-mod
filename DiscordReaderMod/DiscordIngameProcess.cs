using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;

namespace DiscordReaderMod;

public class DiscordIngameProcess
{
    private static Queue<string> _queue = [];
    private static ManualLogSource _log;

    public static void AttachLogger(ManualLogSource logger)
    {
        _log = logger;
    }

    public static void AddTranscribeToQueue(string username, string message)
    {
        string formatted_message = $"{username}: {message}";
        _queue.Enqueue(formatted_message);
        _log.LogInfo("queue entered: " + formatted_message);
    }

    // Process all input and determine based on when in game and promixity range to send message
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    private static void SendTranscribeToHUD(PlayerControllerB playerControllerB)
    {
        _log.LogInfo("SendTranscribeToHUD?");
        if (_queue.Count == 0)
            return;

        if (playerControllerB.isPlayerDead)
            return;

        string current_message = _queue.Dequeue();
        _log.LogInfo("Got queue message to send: " + current_message);

        const string terrariaRed = "#E11919";

        string colored_message = $"<color={terrariaRed}>{current_message}</color>";

        HUDManager.Instance.AddTextToChatOnServer(colored_message);
        _log.LogInfo("Sent transcribe to HUD manager.");
    }
}
