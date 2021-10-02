using System.Collections.Generic;
using Newtonsoft.Json;
using TehGM.Discord.Serialization;

namespace TehGM.Discord.Interactions
{
    public class DiscordInteractionResponse
    {
        public static DiscordInteractionResponse Pong { get; } = new DiscordInteractionResponse(DiscordInteractionResponseType.Pong);

        [JsonProperty("type", Required = Required.Always)]
        public DiscordInteractionResponseType Type { get; private set; }
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordInteractionResponseData Data { get; set; }

        public DiscordInteractionResponse(DiscordInteractionResponseType type)
        {
            this.Type = type;
        }
    }

    public class DiscordInteractionResponseData
    {
        [JsonProperty("tts", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsTTS { get; set; }
        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; set; }
        [JsonProperty("embeds", NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<DiscordEmbed> Embeds { get; set; }
        [JsonProperty("allowed_mentions", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(DiscordAllowedMentionsConverter))]
        public DiscordAllowedMentions AllowedMentions { get; set; }
        [JsonProperty("flags", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordInteractionResponseFlags? Flags { get; set; }

        // TODO: add components (sheesh)
    }
}
