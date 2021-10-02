using System.Collections.Generic;
using Newtonsoft.Json;

namespace TehGM.Discord.Interactions
{
    public class DiscordInteractionResolvedData
    {
        [JsonProperty("users", NullValueHandling = NullValueHandling.Ignore)]
        private Dictionary<ulong, DiscordUser> _users;
        [JsonProperty("members", NullValueHandling = NullValueHandling.Ignore)]
        private Dictionary<ulong, DiscordGuildMember> _members;
        [JsonProperty("messages", NullValueHandling = NullValueHandling.Ignore)]
        private Dictionary<ulong, DiscordMessage> _messages;
        [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
        private Dictionary<ulong, DiscordRole> _roles;
        [JsonProperty("channels", NullValueHandling = NullValueHandling.Ignore)]
        private Dictionary<ulong, DiscordChannel> _channels;

        [JsonIgnore]
        public IReadOnlyDictionary<ulong, DiscordUser> Users => this._users;
        [JsonIgnore]
        public IReadOnlyDictionary<ulong, DiscordGuildMember> GuildMembers => this._members;
        [JsonIgnore]
        public IReadOnlyDictionary<ulong, DiscordMessage> Messages => this._messages;
        [JsonIgnore]
        public IReadOnlyDictionary<ulong, DiscordRole> Roles => this._roles;
        [JsonIgnore]
        public IReadOnlyDictionary<ulong, DiscordChannel> Channels => this._channels;

        [JsonConstructor]
        private DiscordInteractionResolvedData() { }
    }
}
