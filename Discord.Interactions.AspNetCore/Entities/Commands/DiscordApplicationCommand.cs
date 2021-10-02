using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TehGM.Discord.Interactions
{
    public class DiscordApplicationCommand : ICloneable
    {
        public const int MaxNameLength = 32;
        public const int MaxDescriptionLength = 100;

        [JsonProperty("name", Required = Required.Always)]
        private string _name;
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        private string _description;

        // settable
        [JsonProperty("type")]
        public DiscordApplicationCommandType Type { get; set; } = DiscordApplicationCommandType.ChatInput;
        [JsonProperty("default_permission", NullValueHandling = NullValueHandling.Ignore)]
        public bool? EnabledByDefault { get; set; } = true;
        [JsonIgnore]
        public string Name
        {
            get => this._name;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length > MaxNameLength)
                    throw new ArgumentException($"Discord Application Command's {nameof(this.Name)} must be between 1 and {MaxNameLength} characters long.");
                this._name = value;
            }
        }
        [JsonIgnore]
        public string Description
        {
            get => this._description;
            set
            {
                if (this.Type == DiscordApplicationCommandType.ChatInput)
                {
                    if (string.IsNullOrWhiteSpace(value) || value.Length > MaxDescriptionLength)
                        throw new ArgumentException($"Discord Application Command's {nameof(this.Description)} must be between 1 and {MaxDescriptionLength} characters long.");
                }
                else if (!string.IsNullOrEmpty(value))
                    throw new ArgumentException($"Discord Application Commands of type {this.Type} cannot have a description.");
                this._description = value;
            }
        }
        [JsonProperty("options", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ICollection<DiscordApplicationCommandOption> Options { get; set; }

        // readonly
        [JsonProperty("guild_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? GuildID { get; private set; }
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ulong ID { get; private set; }
        [JsonProperty("application_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ulong ApplicationID { get; private set; }
        [JsonProperty("version", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ulong Version { get; private set; }

        [JsonConstructor]
        private DiscordApplicationCommand() { }

        public DiscordApplicationCommand(DiscordApplicationCommandType type, string name, string description, bool enabledByDefault) : this()
        {
            this.Type = type;
            this.Name = name;
            this.Description = description;
            this.EnabledByDefault = enabledByDefault;
        }

        public static DiscordApplicationCommand SlashCommand(string name, string description, bool enabledByDefault = true)
            => new DiscordApplicationCommand(DiscordApplicationCommandType.ChatInput, name, description, enabledByDefault);
        public static DiscordApplicationCommand UserCommand(string name, bool enabledByDefault = true)
            => new DiscordApplicationCommand(DiscordApplicationCommandType.User, name, null, enabledByDefault);
        public static DiscordApplicationCommand MessageCommand(string name, bool enabledByDefault = true)
            => new DiscordApplicationCommand(DiscordApplicationCommandType.Message, name, null, enabledByDefault);

        public object Clone()
        {
            DiscordApplicationCommand result = (DiscordApplicationCommand)this.MemberwiseClone();
            if (this.Options != null)
                result.Options = new List<DiscordApplicationCommandOption>(this.Options.Select(o => (DiscordApplicationCommandOption)o.Clone()));
            return result;
        }
    }
}
