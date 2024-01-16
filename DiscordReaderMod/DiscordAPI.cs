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
    private static string _channel_read;
    // Generic non whitespace username colon any message regex
    private static string message_pattern = @"[\S]+:[\s\S]+";
    private static Regex message_regex = new Regex(message_pattern);

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
        _log.LogInfo("Discord debug on message rec: " + message.ToString());
        // The bot should never respond to itself.
        if (message.Author.Id == _client.CurrentUser.Id)
            return;

        if (message.Content == "!testlethalcompany")
        {
            await message.Channel.SendMessageAsync("Hey there you have successfully tested Lethal Company Discord Bot.");
            return;
        }

        if (message.Content == "!lethalcompanyreader")
        {
            _channel_read = message.Channel.Name;
            await message.Channel.SendMessageAsync("Lethal Company Bot will listen to this channel.");
            return;
        }

        if (message.Channel.Name.Equals(_channel_read))
        {
            await message.Channel.SendMessageAsync("TODO... Heres what I received for: " + message.CleanContent);
            // regex and check if message is valid and should send over to game or not
            if (!message_regex.IsMatch(message.CleanContent))
                return;

            string[] message_content = message.CleanContent.Split(": ");
            var username = message_content[0];
            var content = message_content[1];
            // TODO username mapping here?
            AddTranscribeToQueue(username, content);
            return;
        }

    }

}
