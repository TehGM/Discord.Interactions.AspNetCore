using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TehGM.Discord.Interactions
{
    public class DiscordInteraction
    {
        [JsonProperty("id")]
        public ulong ID { get; private set; }
        [JsonProperty("application_id")]
        public ulong ApplicationID { get; private set; }
        [JsonProperty("type")]
        public DiscordInteractionType Type { get; private set; }
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordInteractionData Data { get; private set; }
        [JsonProperty("guild_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? GuildID { get; private set; }
        [JsonProperty("channel_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? ChannelID { get; private set; }
        [JsonProperty("member", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordGuildMember GuildMember { get; private set; }
        [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordUser User { get; private set; }
        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("version")]
        public int Version { get; private set; }
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordMessage Message { get; private set; }

        [JsonConstructor]
        private DiscordInteraction() { }
    }
}
