using System;
using Newtonsoft.Json;

namespace TehGM.Discord
{
    public class DiscordEmbedMedia
    {
        [JsonProperty("url", Required = Required.Always)]
        private string _url;

        [JsonIgnore]
        public string URL
        {
            get => this._url;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof(this.URL));
                this._url = value;
            }
        }
        [JsonProperty("proxy_url", NullValueHandling = NullValueHandling.Ignore)]
        public string ProxyURL { get; set; }
        [JsonProperty("height", NullValueHandling = NullValueHandling.Ignore)]
        public int? Height { get; set; }
        [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
        public int? Width { get; set; }

        public DiscordEmbedMedia(string url)
        {
            this.URL = url;
        }
    }
}
