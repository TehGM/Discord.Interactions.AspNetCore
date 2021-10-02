using System;
using Newtonsoft.Json;

namespace TehGM.Discord
{
    public class DiscordEmbedAuthor
    {
        public const int MaxNameLength = 256;

        [JsonProperty("name", Required = Required.Always)]
        private string _name;

        [JsonIgnore]
        public string Name
        {
            get => this._name;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof(this.Name));
                if (value.Length > MaxNameLength)
                    throw new InvalidOperationException($"Discord embed author's {nameof(this.Name)} can only have up to {MaxNameLength} characters.");
                this._name = value;
            }
        }
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string URL { get; set; }
        [JsonProperty("icon_url", NullValueHandling = NullValueHandling.Ignore)]
        public string IconURL { get; set; }
        [JsonProperty("proxy_icon_url", NullValueHandling = NullValueHandling.Ignore)]
        public string ProxyIconURL { get; set; }

        public DiscordEmbedAuthor(string name, string url = null, string iconUrl = null)
        {
            this.Name = name;
            this.URL = url;
            this.IconURL = iconUrl;
        }
    }
}
