using System;
using Newtonsoft.Json;

namespace TehGM.Discord
{
    public class DiscordMessage
    {
        [JsonProperty("id")]
        public ulong ID { get; private set; }
        [JsonProperty("channel_id")]
        public ulong ChannelID { get; private set; }
        [JsonProperty("guild_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? GuildID { get; private set; }
        [JsonProperty("author")]
        public DiscordUser Author { get; private set; }
        [JsonProperty("member", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordGuildMember GuildMember { get; private set; }
        [JsonProperty("content")]
        public string Content { get; private set; }
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; private set; }
        [JsonProperty("edited_timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? EditTimestamp { get; private set; }
        [JsonProperty("tts")]
        public bool IsTTS { get; private set; }
        [JsonProperty("mention_everyone")]
        public bool MentionsEveryone { get; private set; }

        // TODO: add other properties - https://discord.com/developers/docs/resources/channel#message-object-message-structure

        [JsonConstructor]
        protected DiscordMessage() { }
    }
}