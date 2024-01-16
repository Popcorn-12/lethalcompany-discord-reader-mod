using BepInEx.Configuration;
namespace DiscordReaderMod;

public class Configuration
{
    public string DiscordBotToken => configDiscordBotToken.Value;
    public string MappingUsers => configMappingUsers.Value;
    public string defaultMappingUsers = "steamUsername=discordUsername";

    private ConfigEntry<string> configDiscordBotToken;
    private ConfigEntry<string> configMappingUsers;
    private ConfigFile File;

    public Configuration(ConfigFile config)
    {
        File = config;
        configDiscordBotToken = File.Bind("General",
            "DiscordBotToken",
            "",
            "Please enter your own discord bot token ('Token' is needed, not public key or application id). Create new bot via Discord Developer Portal");
        configMappingUsers = File.Bind("General",
            "MappingUsers",
            defaultMappingUsers,
            "Please enter mapping users or use !mapuser in discord. Ex: steamUsername=discordUsername;steamUsername2=discordUsername;...");
    }
}
