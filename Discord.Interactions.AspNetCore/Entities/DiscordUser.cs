using Newtonsoft.Json;

namespace TehGM.Discord
{
    public class DiscordUser
    {
        [JsonProperty("id")]
        public ulong ID { get; private set; }
        [JsonProperty("username")]
        public string Username { get; private set; }
        [JsonProperty("discriminator")]
        public string Discriminator { get; private set; }
        [JsonProperty("avatar")]
        public string AvatarHash { get; private set; }
        [JsonProperty("bot", NullValueHandling = NullValueHandling.Ignore)]
        private bool? _bot;
        [JsonIgnore]
        public bool IsBot => this._bot == true;
        [JsonProperty("system", NullValueHandling = NullValueHandling.Ignore)]
        private bool? _system;
        [JsonIgnore]
        public bool IsSystem => this._system == true;
        [JsonProperty("mfa_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsMultiFactorAuthenticationEnabled { get; private set; }
        [JsonProperty("accent_color", NullValueHandling = NullValueHandling.Ignore)]
        public bool? BannerColor { get; private set; }
        [JsonProperty("locale", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; private set; }
        [JsonProperty("flags", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordUserFlags? Flags { get; private set; }
        [JsonProperty("public_flags", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordUserFlags? PublicFlags { get; private set; }
        [JsonProperty("premium_type", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordPremiumType? PremiumType { get; private set; }

        [JsonConstructor]
        protected DiscordUser() { }
    }

    [System.Flags]
    public enum DiscordUserFlags
    {
        None = 0,
        DiscordEmployee = 1 << 0,
        PartneredServerOwner = 1 << 1,
        HypeSquadEvents = 1 << 2,
        BugHunterLevel1 = 1 << 3,
        HouseBravery = 1 << 6,
        HouseBrilliance = 1 << 7,
        HouseBalance = 1 << 8,
        EarlySupporter = 1 << 9,
        TeamUser = 1 << 10,
        BugHunterLevel2 = 1 << 14,
        VerifiedBot = 1 << 16,
        EarlyVerifiedBotDeveloper = 1 << 17,
        DiscordCertifiedModerator = 1 << 18
    }
}
