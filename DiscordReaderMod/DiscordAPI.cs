using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using BepInEx.Logging;
using System.Text.RegularExpressions;
using static DiscordReaderMod.DiscordIngameProcess;

namespace DiscordReaderMod;

public class DiscordAPI
{
    private static DiscordSocketClient _client;
    private static ManualLogSource _log;
    private static string _channelRead;
    // Generic non whitespace username colon any message regex
    private static string messagePattern = @"[\S]+:[\s\S]+";
    private static Regex messageRegex = new Regex(messagePattern);
    private static string mapUserHelpMsg = "Please enter steamUserName and discordUsername. Ex: !lcmapuser steamUsername=discordUsername";

    public static async Task Main(ManualLogSource Log)
    {
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };

        _client = new DiscordSocketClient(config);

        _log = Log;

        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.MessageReceived += MessageReceivedAsync;

        // Tokens should be considered secret data, and never hard-coded.
        await _client.LoginAsync(TokenType.Bot, Plugin.configuration.DiscordBotToken);

        await _client.StartAsync();

        AttachLogger(Log);
    }

    public static async Task Shutdown()
    {
        // TODO Find a way to properly shutdown discord bot
        // Need to find hook to exit and call this method
        if (_client != null && _client.ConnectionState.Equals(ConnectionState.Connected))
        {
            _client.Dispose();
            _client = null;
            return;
        }
    }

    private static Task LogAsync(LogMessage log)
    {
        _log.LogInfo("Discord Log: " + log.ToString());

        return Task.CompletedTask;
    }

    private static Task ReadyAsync()
    {
        _log.LogInfo($"Discord Log: {_client.CurrentUser} is connected!");

        return Task.CompletedTask;
    }

    private static async Task MessageReceivedAsync(SocketMessage message)
    {
        _log.LogDebug("Discord debug on message rec: " + message.ToString());
        // The bot should never respond to itself.
        if (message.Author.Id == _client.CurrentUser.Id)
            return;

        if (message.Content == "!lctest")
        {
            await message.Channel.SendMessageAsync("Hey there you have successfully tested Lethal Company Discord Bot.");
            return;
        }

        if (message.Content == "!lcreader")
        {
            _channelRead = message.Channel.Name;
            await message.Channel.SendMessageAsync("Lethal Company Bot will listen to this channel.");
            return;
        }

        if (message.Content == "!lcmapuser")
        {
            await message.Channel.SendMessageAsync(mapUserHelpMsg);
        }
        else if (message.Content.StartsWith("!lcmapuser"))
        {
            try
            {
                string[] getMessage = message.Content.Split(" ");
                if (getMessage.Length != 2 && getMessage[0].Equals("!lcmapuser"))
                {
                    await message.Channel.SendMessageAsync(mapUserHelpMsg);
                    return;
                }
                string[] userlist = getMessage[1].Split("=");
                if (userlist.Length == 2)
                {
                    string steamUsername = userlist[0].Trim().ToLower();
                    string discordUsername = userlist[1].Trim().ToLower();
                    AddUsernameToList(steamUsername, discordUsername);
                    await message.Channel.SendMessageAsync($"Entered to list for steam: {steamUsername} mapped to {discordUsername}");
                }
            }
            catch
            {
                await message.Channel.SendMessageAsync(mapUserHelpMsg);
            }
            return;
        }

        if (message.Channel.Name.Equals(_channelRead))
        {
            // regex and check if message is valid and should send over to game or not
            if (!messageRegex.IsMatch(message.CleanContent))
                return;

            string[] message_content = message.CleanContent.Split(": ");
            var username = message_content[0];
            var content = message_content[1];
            AddTranscribeToQueue(username, content);
            return;
        }

    }

}
