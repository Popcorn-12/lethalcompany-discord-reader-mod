using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordReaderMod
{
    internal class PlayerMessage
    {

        public PlayerMessage(string username, string message)
        {
            this.username = username;
            this.message = message;
        }

        public string username { get; set; }
        public string message { get; set; }

        public string FormattedMessage()
        {
            return $"{username}: {message}";
        }
    }
}
