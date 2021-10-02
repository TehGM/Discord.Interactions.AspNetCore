using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TehGM.Discord.Interactions
{
    public class DiscordApplicationCommandOption : ICloneable
    {
        public const int MaxNameLength = 32;
        public const int MaxDescriptionLength = 100;

        [JsonProperty("name", Required = Required.Always)]
        private string _name;
        [JsonProperty("description", Required = Required.Always)]
        private string _description;

        [JsonIgnore]
        public string Name
        {
            get => this._name;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length > MaxNameLength)
                    throw new ArgumentException($"Discord Application Command Option's {nameof(this.Name)} must be between 1 and {MaxNameLength} characters long.");
                this._name = value;
            }
        }
        [JsonIgnore]
        public string Description
        {
            get => this._description;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length > MaxDescriptionLength)
                    throw new ArgumentException($"Discord Application Command Option's {nameof(this.Description)} must be between 1 and {MaxDescriptionLength} characters long.");
                this._description = value;
            }
        }
        [JsonProperty("type")]
        public DiscordApplicationCommandOptionType Type { get; set; }
        [JsonProperty("required", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsRequired { get; set; }
        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<DiscordApplicationCommandOption> NestedOptions { get; set; }
        [JsonProperty("choices", NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<DiscordApplicationCommandOptionChoice> Choices { get; set; }

        [JsonConstructor]
        private DiscordApplicationCommandOption() { }

        public DiscordApplicationCommandOption(DiscordApplicationCommandOptionType type, string name, string description)
        {
            this.Type = type;
            this.Name = name;
            this.Description = description;
        }

        public object Clone()
        {
            DiscordApplicationCommandOption result = (DiscordApplicationCommandOption)this.MemberwiseClone();
            if (this.NestedOptions != null)
                result.NestedOptions = new List<DiscordApplicationCommandOption>(this.NestedOptions.Select(no => (DiscordApplicationCommandOption)no.Clone()));
            if (this.Choices != null)
                result.Choices = new List<DiscordApplicationCommandOptionChoice>(this.Choices.Select(c => (DiscordApplicationCommandOptionChoice)c.Clone()));
            return result;
        }
    }

    public enum DiscordApplicationCommandOptionType
    {
        SubCommand = 1,
        SubCommandGroup = 2,
        String = 3,
        /// <summary>Int32.</summary>
        Integer = 4,
        Boolean = 5,
        User = 6,
        Channel = 7,
        Role = 8,
        /// <summary>Users and roles.</summary>
        Mentionable = 9,
        /// <summary>Double, Int32 range.</summary>
        Number = 10
    }
}
