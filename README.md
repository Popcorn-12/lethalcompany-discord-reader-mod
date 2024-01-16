# Discord Reader Mod
Lethal Company Mod runs a Discord bot to listen and read channel message to dump out messages to ingame chatbox.

This is geared toward to Deaf/HoH people who uses transcribing bots in discord which translate speech to text on a Discord server but wants full immersion of Lethal Company. This mod specifically will send messages via player's chat box system including proximity range.

# Current expectation of message for bot to pick up message in Discord channel from transcribe bots
```<username>: <message>```

Ex: ```someuser: Hey watch out for that Bracken!```

# Recommended Mod
To improve readability within chatbox especially when transcribe get long and talking is usually faster than typing.
https://thunderstore.io/c/lethal-company/p/taffyko/NiceChat/

# Instruction to run this mod
To get start with this mod, you will need a Discord bot token which can be obtained from Discord developer portal: https://discord.com/developers

Run the Lethal company with mod installed at least once to generate config file. Then put your Discord bot token in the config file. With r2modman/thunderstore mod manager, you can find this "DiscordReaderMod" config file under "Config Editor" on left side menu. For manual install, edit config file within game folder.

Add your desired hardcoded Mapping Users, this is usually required because discord username is not same as steam username. Modify this in "MappingUsers" in config file or run command within Discord server to map each user

Launch the game and you should see "DiscordReaderMod] Discord Log: <your_bot_name>#1234 is connected!" To confirm your Discord bot token is inserted correctly and running bot as well

Add your bot to your own server

# Commands on Discord server
```!lctest``` - runs command to check if bot is accessing the Discord server and can read your command

```!lcreader``` - Tells bot to listen to this specific channel within your Discord server

```!lcmapuser``` - Must map each user within discord for bot to translate and route to ingame player. Type '!lcmapuser' for details to how do it

# Future Roadmap
* Add some kind of ability in configuration where output of transcribe bot might not be consistent to include several way of mapping to extract username and message.
* Customize transcribe color ingame
* Add more configs (such as allowing transcribe to be sent despite if player is dead, etc)
