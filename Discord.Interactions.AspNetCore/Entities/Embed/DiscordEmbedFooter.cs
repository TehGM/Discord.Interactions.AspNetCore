using System;
using Newtonsoft.Json;

namespace TehGM.Discord
{
    public class DiscordEmbedFooter
    {
        public const int MaxTextLength = 2048;

        [JsonProperty("text", Required = Required.Always)]
        private string _text;

        [JsonIgnore]
        public string Text
        {
            get => this._text;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof(this.Text));
                if (value.Length > MaxTextLength)
                    throw new InvalidOperationException($"Discord embed footer's {nameof(this.Text)} can only have up to {MaxTextLength} characters.");
                this._text = value;
            }
        }
        [JsonProperty("icon_url", NullValueHandling = NullValueHandling.Ignore)]
        public string IconURL { get; set; }
        [JsonProperty("proxy_icon_url", NullValueHandling = NullValueHandling.Ignore)]
        public string ProxyIconURL { get; set; }

        public DiscordEmbedFooter(string text, string iconUrl = null)
        {
            this.Text = text;
            this.IconURL = iconUrl;
        }
    }
}
