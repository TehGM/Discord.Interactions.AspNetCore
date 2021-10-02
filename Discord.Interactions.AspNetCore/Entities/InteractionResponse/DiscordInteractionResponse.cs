using Newtonsoft.Json;

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
}
