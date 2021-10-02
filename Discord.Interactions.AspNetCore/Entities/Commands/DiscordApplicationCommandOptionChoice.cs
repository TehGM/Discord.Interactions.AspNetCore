using System;
using Newtonsoft.Json;

namespace TehGM.Discord.Interactions
{
    public class DiscordApplicationCommandOptionChoice : ICloneable
    {
        public const int MaxNameLength = 100;
        public const int MaxValueLength = 100;

        [JsonProperty("name", Required = Required.Always)]
        private string _name;
        [JsonProperty("value", Required = Required.Always)]
        private object _value;

        [JsonIgnore]
        public string Name
        {
            get => this._name;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length > MaxNameLength)
                    throw new ArgumentException($"Discord Application Command Option Choice's {nameof(this.Name)} must be between 1 and {MaxNameLength} characters long.");
                this._name = value;
            }
        }
        [JsonIgnore]
        public object Value
        {
            get => this._value;
            private set
            {
                if (value is string s && (string.IsNullOrEmpty(s) || s.Length > MaxValueLength))
                    throw new ArgumentException($"Discord Application Command Option Choice's {nameof(this.Value)} must be between 1 and {MaxValueLength} characters long.");
                this._value = value;
            }
        }

        public void SetValue(int integerValue)
            => this.Value = integerValue;
        public void SetValue(double numberValue)
            => this.Value = numberValue;
        public void SetValue(string stringValue)
            => this.Value = stringValue;

        [JsonConstructor]
        private DiscordApplicationCommandOptionChoice() { }

        private DiscordApplicationCommandOptionChoice(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public static DiscordApplicationCommandOptionChoice IntegerChoice(string name, int value)
            => new DiscordApplicationCommandOptionChoice(name, value);
        public static DiscordApplicationCommandOptionChoice NumberChoice(string name, double value)
            => new DiscordApplicationCommandOptionChoice(name, value);
        public static DiscordApplicationCommandOptionChoice StringChoice(string name, string value)
            => new DiscordApplicationCommandOptionChoice(name, value);

        public object Clone()
        {
            DiscordApplicationCommandOptionChoice result = (DiscordApplicationCommandOptionChoice)this.MemberwiseClone();
            return result;
        }
    }
}
