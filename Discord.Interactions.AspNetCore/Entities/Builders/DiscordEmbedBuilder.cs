using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Discord
{
    public class DiscordEmbedBuilder
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DiscordEmbedType? Type { get; set; }
        public string URL { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public int? Color { get; set; }
        public DiscordEmbedFooter Footer { get; set; }
        public DiscordEmbedMedia Image { get; set; }
        public DiscordEmbedMedia Thumbnail { get; set; }
        public DiscordEmbedMedia Video { get; set; }
        public DiscordEmbedProvider Provider { get; set; }
        public DiscordEmbedAuthor Author { get; set; }
        public ICollection<DiscordEmbedField> Fields { get; set; }

        public DiscordEmbedBuilder()
        {
            this.Type = DiscordEmbedType.Rich;
        }

        private DiscordEmbedBuilder Modify(Action<DiscordEmbedBuilder> builder)
        {
            builder.Invoke(this);
            return this;
        }

        public DiscordEmbedBuilder WithTitle(string title)
            => this.Modify(embed => embed.Title = title);
        public DiscordEmbedBuilder WithDescription(string description)
            => this.Modify(embed => embed.Description = description);
        public DiscordEmbedBuilder WithType(DiscordEmbedType? type)
            => this.Modify(embed => embed.Type = type);
        public DiscordEmbedBuilder WithURL(string url)
            => this.Modify(embed => embed.URL = url);
        public DiscordEmbedBuilder WithColor(int? color)
            => this.Modify(embed => embed.Color = color);

        public DiscordEmbedBuilder WithTimestamp(DateTimeOffset? timestamp)
            => this.Modify(embed => embed.Timestamp = timestamp);
        public DiscordEmbedBuilder WithTimestamp(DateTime? timestamp)
            => this.WithTimestamp(timestamp.HasValue ? (DateTimeOffset?)new DateTimeOffset(timestamp.Value) : null);
        public DiscordEmbedBuilder WithCurrentTimestamp()
            => this.WithTimestamp(DateTimeOffset.UtcNow);

        public DiscordEmbedBuilder WithFooter(DiscordEmbedFooter footer)
            => this.Modify(embed => embed.Footer = footer);
        public DiscordEmbedBuilder WithFooter(string text, string iconURL = null, string proxyIconURL = null)
            => this.WithFooter(new DiscordEmbedFooter(text, iconURL) { ProxyIconURL = proxyIconURL });

        public DiscordEmbedBuilder WithImage(DiscordEmbedMedia image)
            => this.Modify(embed => embed.Image = image);
        public DiscordEmbedBuilder WithImage(string imageURL, string proxyImageURL = null)
            => this.WithImage(new DiscordEmbedMedia(imageURL) { ProxyURL = proxyImageURL });

        public DiscordEmbedBuilder WithThumbnail(DiscordEmbedMedia thumbnail)
            => this.Modify(embed => embed.Thumbnail = thumbnail);
        public DiscordEmbedBuilder WithThumbnail(string thumbnailURL, string proxyThumbnailURL = null)
            => this.WithThumbnail(new DiscordEmbedMedia(thumbnailURL) { ProxyURL = proxyThumbnailURL });

        public DiscordEmbedBuilder WithAuthor(DiscordEmbedAuthor author)
            => this.Modify(embed => embed.Author = author);
        public DiscordEmbedBuilder WithAuthor(string name, string url, string iconURL = null, string proxyIconURL = null)
            => this.WithAuthor(new DiscordEmbedAuthor(name, url, iconURL) { ProxyIconURL = proxyIconURL });

        public DiscordEmbedBuilder AddField(DiscordEmbedField field)
        {
            if (this.Fields == null)
                this.Fields = new List<DiscordEmbedField>();
            this.Fields.Add(field);
            return this;
        }

        public DiscordEmbedBuilder AddField(string name, string value, bool? inline = null)
            => this.AddField(new DiscordEmbedField(name, value, inline));

        public DiscordEmbed Build()
        {
            DiscordEmbed result = new DiscordEmbed();
            result.Title = this.Title;
            result.Description = this.Description;
            result.Type = this.Type;
            result.URL = this.URL;
            result.Timestamp = this.Timestamp;
            result.Color = this.Color;
            result.Footer = this.Footer;
            result.Image = this.Image;
            result.Thumbnail = this.Thumbnail;
            result.Video = this.Video;
            result.Provider = this.Provider;
            result.Author = this.Author;
            if (this.Fields?.Any() == true)
                result.Fields = this.Fields;
            return result;
        }
    }
}
