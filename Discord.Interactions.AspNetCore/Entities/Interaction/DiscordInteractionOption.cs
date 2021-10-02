using Newtonsoft.Json;

namespace TehGM.Discord.Interactions
{
    public class DiscordInteractionOption
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; private set; }
        [JsonProperty("value", Required = Required.AllowNull)]
        public object Value { get; private set; }

        [JsonConstructor]
        private DiscordInteractionOption() { }
    }
}
