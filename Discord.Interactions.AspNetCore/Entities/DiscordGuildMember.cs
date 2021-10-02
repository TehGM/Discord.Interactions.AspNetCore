using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TehGM.Discord
{
    public class DiscordGuildMember
    {
        [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordUser User { get; private set; }
        [JsonProperty("nick", NullValueHandling = NullValueHandling.Ignore)]
        public string Nickname { get; private set; }
        [JsonProperty("roles")]
        public IEnumerable<ulong> RoleIDs { get; private set; }
        [JsonProperty("joined_at")]
        public DateTime JoinedTime { get; private set; }
        [JsonProperty("premium_since", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? PremiumSince { get; private set; }
        [JsonProperty("deaf")]
        public bool IsDeafened { get; private set; }
        [JsonProperty("mute")]
        public bool IsMuted { get; private set; }
        [JsonProperty("pending", NullValueHandling = NullValueHandling.Ignore)]
        private bool? _pending;
        [JsonIgnore]
        public bool IsPending => this._pending == true;
        [JsonProperty("permissions", NullValueHandling = NullValueHandling.Ignore)]
        public string Permissions { get; private set; }

        [JsonConstructor]
        protected DiscordGuildMember() { }
    }
}
