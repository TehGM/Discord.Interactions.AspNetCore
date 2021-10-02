using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Discord.Interactions
{
    public class DiscordInteractionResponseBuilder
    {
        public DiscordInteractionResponseType Type { get; set; }
        public bool IsTTS { get; set; }
        public string Content { get; set; }
        public ICollection<DiscordEmbed> Embeds { get; set; }
        public DiscordAllowedMentions AllowedMentions { get; set; }
        public DiscordInteractionResponseFlags? Flags { get; set; }

        public DiscordInteractionResponseBuilder()
        {
            this.Type = DiscordInteractionResponseType.ChannelMessageWithSource;
        }

        private DiscordInteractionResponseBuilder Modify(Action<DiscordInteractionResponseBuilder> builder)
        {
            builder.Invoke(this);
            return this;
        }

        public DiscordInteractionResponseBuilder WithType(DiscordInteractionResponseType type)
            => this.Modify(builder => builder.Type = type);
        public DiscordInteractionResponseBuilder WithTTS(bool tts)
            => this.Modify(builder => builder.IsTTS = tts);
        public DiscordInteractionResponseBuilder WithText(string text)
            => this.Modify(builder => builder.Content = text);
        public DiscordInteractionResponseBuilder WithAllowedMentions(DiscordAllowedMentions mentions)
            => this.Modify(builder => builder.AllowedMentions = mentions);

        public DiscordInteractionResponseBuilder WithFlags(DiscordInteractionResponseFlags? flags)
            => this.Modify(builder => builder.Flags = flags);
        public DiscordInteractionResponseBuilder WithEphemeral(bool ephemeral = true)
            => this.WithFlags(ephemeral ? (DiscordInteractionResponseFlags?)DiscordInteractionResponseFlags.Ephemeral : null);

        public DiscordInteractionResponseBuilder AddEmbed(DiscordEmbed embed)
        {
            if (this.Embeds == null)
                this.Embeds = new List<DiscordEmbed>();
            this.Embeds.Add(embed);
            return this;
        }
        public DiscordInteractionResponseBuilder AddEmbed(Action<DiscordEmbedBuilder> configure)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            configure.Invoke(builder);
            return this.AddEmbed(builder.Build());
        }

        public DiscordInteractionResponse Build()
        {
            DiscordInteractionResponse result = new DiscordInteractionResponse(this.Type);

            // determine if Data element needs to be created
            if (this.IsTTS || !string.IsNullOrEmpty(this.Content) || this.Embeds?.Any() == true ||
                this.AllowedMentions != null || this.Flags != null)
            {
                result.Data = new DiscordInteractionResponseData();
                result.Data.IsTTS = this.IsTTS;
                result.Data.Content = this.Content;
                result.Data.AllowedMentions = this.AllowedMentions;
                result.Data.Flags = this.Flags;
                if (this.Embeds?.Any() == true)
                    result.Data.Embeds = this.Embeds;
            }

            return result;
        }
    }
}
