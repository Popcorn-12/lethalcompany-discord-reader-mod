using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DiscordReaderMod;

public class DiscordIngameProcess
{
    private static Queue<PlayerMessage> _queue = [];
    private static ManualLogSource _log;
    private static float lastUpdateTime = 0;
    private static Dictionary<string, string> playerMapping = new Dictionary<string, string>();
    const string whiteColor = "#FFFFFF";
    const string transcribeColor = "#7bf542";

    public static void AttachLogger(ManualLogSource logger)
    {
        _log = logger;
    }

    public static void LoadUsernameFromConfig(string userConfig)
    {
        string[] listUsers = userConfig.Split(';');
        foreach (string user in listUsers)
        {
            try
            {
                string steamUsername = user.Split("=")[0].Trim().ToLower();
                string discordUsername = user.Split("=")[1].Trim().ToLower();
                AddUsernameToList(steamUsername, discordUsername);
            } catch
            {
                _log.LogError($"Failed to map user for following string: {user}");
            }
        }
        _log.LogInfo("Map user loaded with: \n" + string.Join("\n", playerMapping));
    }

    public static void AddUsernameToList(string steamUsername, string discordUsername)
    {
        if (playerMapping.ContainsKey(steamUsername))
        {
            playerMapping[discordUsername] = steamUsername;
        }
        else
        {
            playerMapping.Add(discordUsername, steamUsername);
        }
    }

    public static void AddTranscribeToQueue(string username, string message)
    {
        PlayerMessage playerMessage = new PlayerMessage(username, message);
        _queue.Enqueue(playerMessage);
        _log.LogInfo("queue entered: " + playerMessage.formatted_message());
    }

    // Process all input and determine based on when in game and promixity range to send message
    [HarmonyPatch(typeof(PlayerControllerB), "Update"), HarmonyPostfix]
    private static void SendTranscribeToHUD(PlayerControllerB __instance)
    {
        // Might want to clear out queue because it has been since 30 secs to avoid pop up all message at once
        if (lastUpdateTime + 30 < Time.time)
        {
            _log.LogInfo("Clearing out transcribe queue");
            _queue.Clear();
        }

        lastUpdateTime = Time.time;

        if (_queue.Count == 0)
            return;

        PlayerMessage current_message = _queue.Dequeue();
        _log.LogInfo("Got queue message to send: " + current_message.formatted_message());

        var getPlayerId = -2;
        string getUsername = "";
        
        // Check if username is in mapping or not
        if (!playerMapping.TryGetValue(current_message.username.ToLower(), out getUsername))
        {
            string system_message = $"<color={whiteColor}>Please use !lcmapuser in discord for {current_message.username}</color>";
            HUDManager.Instance.AddTextToChatOnServer(system_message);
            return;
        }

        PlayerControllerB component = null;
        for (int i = 0; i < __instance.playersManager.allPlayerObjects.Length; i++)
        {
            component = __instance.playersManager.allPlayerObjects[i].GetComponent<PlayerControllerB>();
            if (component != null)
            {
                _log.LogInfo($"Iterating in player list. playerUsername: {component.playerUsername}");
                if (component.playerUsername.ToLower().Equals(getUsername))
                {
                    getPlayerId = i;
                    break;
                }
            }
        }

        if (getPlayerId == -2)
        {
            string system_message = $"<color={whiteColor}>Please use !lcmapuser in discord for {current_message.username} or player is not found ingame</color>";
            HUDManager.Instance.AddTextToChatOnServer(system_message);
            return;
        }
        else
        {
            // Can't send transcribe for dead player
            if (component != null && component.isPlayerDead)
                return;
            string colored_message = $"<color={transcribeColor}>{current_message.message}</color>";
            HUDManager.Instance.AddTextToChatOnServer(colored_message, getPlayerId);
        }
        
        _log.LogInfo($"Sent transcribe to HUD manager for {current_message.username}");
    }



}
