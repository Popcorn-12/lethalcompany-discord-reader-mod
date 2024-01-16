using BepInEx.Configuration;
namespace DiscordReaderMod;

public class Configuration
{
    public string DiscordBotToken => configDiscordBotToken.Value;

    private ConfigEntry<string> configDiscordBotToken;
    private ConfigFile File;

    public Configuration(ConfigFile config)
    {
        File = config;
        configDiscordBotToken = File.Bind("General",
            "DiscordBotToken",
            "",
            "Please enter your own discord bot token ('Token' is needed, not public key or application id'). Create new bot via Discord Developer Portal");
    }
}
