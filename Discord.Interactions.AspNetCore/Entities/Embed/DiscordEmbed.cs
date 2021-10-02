using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TehGM.Discord
{
    public class DiscordEmbed
    {
        public const int MaxTitleLength = 256;
        public const int MaxDescriptionLength = 4096;
        public const int MaxFieldsCount = 25;

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        private string _title;
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        private string _description;
        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        private ICollection<DiscordEmbedField> _fields;

        [JsonIgnore]
        public string Title
        {
            get => this._title;
            set
            {
                if (value != null && value.Length > MaxTitleLength)
                    throw new InvalidOperationException($"Discord embed's {nameof(this.Title)} can only have up to {MaxTitleLength} characters.");
                this._title = value;
            }
        }
        [JsonIgnore]
        public string Description
        {
            get => this._description;
            set
            {
                if (value != null && value.Length > MaxDescriptionLength)
                    throw new InvalidOperationException($"Discord embed's {nameof(this.Description)} can only have up to {MaxDescriptionLength} characters.");
                this._description = value;
            }
        }
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter), new object[] { true })]
        public DiscordEmbedType? Type { get; set; } = DiscordEmbedType.Rich;
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string URL { get; set; }
        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Timestamp { get; set; }
        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public int? Color { get; set; }
        [JsonProperty("footer", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordEmbedFooter Footer { get; set; }
        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordEmbedMedia Image { get; set; }
        [JsonProperty("thumbnail", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordEmbedMedia Thumbnail { get; set; }
        [JsonProperty("video", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordEmbedMedia Video { get; set; }
        [JsonProperty("provider", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordEmbedProvider Provider { get; set; }
        [JsonProperty("author", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordEmbedAuthor Author { get; set; }
        [JsonIgnore]
        public ICollection<DiscordEmbedField> Fields
        {
            get => this._fields;
            set
            {
                if (value != null && value.Count > MaxFieldsCount)
                    throw new InvalidOperationException($"Discord embed only supports {MaxFieldsCount} fields.");
                this._fields = value;
            }
        }

        public void AddField(DiscordEmbedField field)
        {
            if (this.Fields == null)
                this.Fields = new List<DiscordEmbedField>();
            if (this.Fields.Count >= MaxFieldsCount)
                throw new InvalidOperationException($"Discord embed only supports {MaxFieldsCount} fields.");
            this.Fields.Add(field);
        }

        public void AddField(string name, string value, bool? inline = null)
            => this.AddField(new DiscordEmbedField(name, value, inline));
    }

}
