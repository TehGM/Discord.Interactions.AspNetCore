using System;
using Newtonsoft.Json;

namespace TehGM.Discord
{
    public class DiscordEmbedField
    {
        public const int MaxNameLength = 256;
        public const int MaxValueLength = 1024;

        [JsonProperty("name", Required = Required.Always)]
        private string _name;
        [JsonProperty("value", Required = Required.Always)]
        private string _value;

        [JsonIgnore]
        public string Name
        {
            get => this._name;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof(this.Name));
                if (value.Length > MaxNameLength)
                    throw new InvalidOperationException($"Discord embed field's {nameof(this.Name)} can only have up to {MaxNameLength} characters.");
                this._name = value;
            }
        }
        [JsonIgnore]
        public string Value
        {
            get => this._value;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof(this.Value));
                if (value.Length > MaxValueLength)
                    throw new InvalidOperationException($"Discord embed field's {nameof(this.Value)} can only have up to {MaxValueLength} characters.");
                this._value = value;
            }
        }
        [JsonProperty("inline", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsInline { get; set; }

        public DiscordEmbedField(string name, string value, bool? inline = null)
        {
            this.Name = name;
            this.Value = value;
            this.IsInline = inline;
        }
    }
}
