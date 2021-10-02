﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TehGM.Discord.Interactions
{
    public class DiscordInteractionData
    {
        [JsonProperty("id")]
        public ulong ID { get; private set; }
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("type")]
        public DiscordApplicationCommandType CommandType { get; private set; }
        [JsonProperty("target_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? TargetID { get; private set; }

        [JsonProperty("resolved", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordInteractionResolvedData ResolvedData { get; private set; }
        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<DiscordInteractionDataOption> Options { get; private set; }

        [JsonConstructor]
        private DiscordInteractionData() { }

        // OPTIONS RETRIEVAL
        private bool TryGetOption<T>(string key, out T value, Func<object, T> parser)
        {
            DiscordInteractionDataOption option = this.Options.FirstOrDefault(o => o.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (option == null || option.Value == null)
            {
                value = default;
                return false;
            }
            value = parser(option.Value);
            return true;
        }
        public bool TryGetStringOption(string key, out string value)
            => this.TryGetOption(key, out value, obj => obj.ToString());
        public bool TryGetIntegerOption(string key, out int value)
            => this.TryGetOption(key, out value, obj => int.Parse(obj.ToString()));
        public bool TryGetNumberOption(string key, out double value)
            => this.TryGetOption(key, out value, obj => double.Parse(obj.ToString()));
        public bool TryGetBooleanOption(string key, out bool value)
            => this.TryGetOption(key, out value, obj => bool.Parse(obj.ToString()));

        // RESOLVED RETRIEVAL
        private bool TryGetResolved<T>(ulong id, out T value, IReadOnlyDictionary<ulong, T> dictionary)
        {
            if (dictionary == null)
            {
                value = default;
                return false;
            }
            return dictionary.TryGetValue(id, out value);
        }
        public bool TryGetUser(ulong id, out DiscordUser user)
            => this.TryGetResolved(id, out user, this.ResolvedData?.Users);
        public bool TryGetGuildMember(ulong id, out DiscordGuildMember user)
            => this.TryGetResolved(id, out user, this.ResolvedData?.GuildMembers);
        public bool TryGetChannel(ulong id, out DiscordChannel user)
            => this.TryGetResolved(id, out user, this.ResolvedData?.Channels);
        public bool TryGetMessage(ulong id, out DiscordMessage user)
            => this.TryGetResolved(id, out user, this.ResolvedData?.Messages);
        public bool TryGetRole(ulong id, out DiscordRole user)
            => this.TryGetResolved(id, out user, this.ResolvedData?.Roles);
    }
}