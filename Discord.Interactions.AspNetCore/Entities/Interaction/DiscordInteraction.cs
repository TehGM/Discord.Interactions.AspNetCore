﻿using Newtonsoft.Json;

namespace TehGM.Discord.Interactions
{
    /// <summary>Represents a received Discord interaction.</summary>
    /// <seealso href="https://discord.com/developers/docs/interactions/receiving-and-responding#interactions"/>
    public class DiscordInteraction
    {
        /// <summary>id of the interaction</summary>
        [JsonProperty("id")]
        public ulong ID { get; private set; }
        /// <summary>id of the application this interaction is for</summary>
        [JsonProperty("application_id")]
        public ulong ApplicationID { get; private set; }
        /// <summary>the type of interaction</summary>
        [JsonProperty("type")]
        public DiscordInteractionType Type { get; private set; }
        /// <summary>the command data payload</summary>
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordInteractionData Data { get; private set; }
        /// <summary>the guild it was sent from</summary>
        [JsonProperty("guild_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? GuildID { get; private set; }
        /// <summary>the channel it was sent from</summary>
        [JsonProperty("channel_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? ChannelID { get; private set; }
        /// <summary>guild member data for the invoking user, including permissions</summary>
        [JsonProperty("member", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordGuildMember GuildMember { get; private set; }
        /// <summary>user object for the invoking user, if invoked in a DM</summary>
        [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordUser User { get; private set; }
        /// <summary>a continuation token for responding to the interaction</summary>
        [JsonProperty("token")]
        public string Token { get; private set; }
        /// <summary>read-only property, always 1</summary>
        [JsonProperty("version")]
        public int Version { get; private set; }
        /// <summary>for components, the message they were attached to</summary>
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordMessage Message { get; private set; }

        /// <summary>Creates a new instance of this class.</summary>
        /// <remarks>This constructor exists for JSON deserialization.</remarks>
        [JsonConstructor]
        protected DiscordInteraction() { }
    }
}
