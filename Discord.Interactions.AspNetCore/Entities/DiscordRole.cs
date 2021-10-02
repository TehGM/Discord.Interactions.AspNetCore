using Newtonsoft.Json;

namespace TehGM.Discord
{
    public class DiscordRole
    {
        [JsonProperty("id", Required = Required.Always)]
        public ulong ID { get; private set; }
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; private set; }
        [JsonProperty("color", Required = Required.Always)]
        public int Color { get; private set; }
        [JsonProperty("hoist", Required = Required.Always)]
        public bool IsPinned { get; private set; }
        [JsonProperty("position", Required = Required.Always)]
        public int Position { get; private set; }
        [JsonProperty("permissions", Required = Required.Always)]
        public ulong Permissions { get; private set; }
        [JsonProperty("managed", Required = Required.Always)]
        public bool IsManaged { get; private set; }
        [JsonProperty("mentionable", Required = Required.Always)]
        public bool IsMentionable { get; private set; }
        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordRoleTags Tags { get; private set; }

        [JsonConstructor]
        protected DiscordRole() { }
    }

    public class DiscordRoleTags
    {
        [JsonProperty("bot_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? BotID { get; private set; }
        [JsonProperty("integration_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? IntegrationID { get; private set; }
        // premium_subscriber is of type null according to docs????
        [JsonProperty("premium_subscriber", NullValueHandling = NullValueHandling.Ignore)]
        public object PremiumSubscriber { get; private set; }

        [JsonConstructor]
        protected DiscordRoleTags() { }
    }
}
