using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Discord.Interactions
{
    public class DiscordApplicationCommandOptionBuilder
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DiscordApplicationCommandOptionType Type { get; set; }
        public bool IsRequired { get; set; }
        public ICollection<DiscordApplicationCommandOption> NestedOptions { get; set; }
        public ICollection<DiscordApplicationCommandOptionChoice> Choices { get; set; }

        public DiscordApplicationCommandOptionBuilder() { }

        private DiscordApplicationCommandOptionBuilder Modify(Action<DiscordApplicationCommandOptionBuilder> builder)
        {
            builder.Invoke(this);
            return this;
        }

        public DiscordApplicationCommandOptionBuilder WithName(string name)
            => this.Modify(opt => opt.Name = name);
        public DiscordApplicationCommandOptionBuilder WithDescription(string description)
            => this.Modify(opt => opt.Description = description);
        public DiscordApplicationCommandOptionBuilder WithType(DiscordApplicationCommandOptionType type)
            => this.Modify(opt => opt.Type = type);
        public DiscordApplicationCommandOptionBuilder WithRequired(bool required)
            => this.Modify(opt => opt.IsRequired = required);

        public DiscordApplicationCommandOptionBuilder AddNestedOption(DiscordApplicationCommandOption option)
        {
            if (this.NestedOptions == null)
                this.NestedOptions = new List<DiscordApplicationCommandOption>();
            this.NestedOptions.Add(option);
            return this;
        }
        public DiscordApplicationCommandOptionBuilder AddNestedOption(Action<DiscordApplicationCommandOptionBuilder> configure)
        {
            DiscordApplicationCommandOptionBuilder builder = new DiscordApplicationCommandOptionBuilder();
            configure.Invoke(builder);
            return this.AddNestedOption(builder.Build());
        }

        public DiscordApplicationCommandOptionBuilder AddChoice(DiscordApplicationCommandOptionChoice choice)
        {
            if (this.Choices == null)
                this.Choices = new List<DiscordApplicationCommandOptionChoice>();
            this.Choices.Add(choice);
            return this;
        }
        public DiscordApplicationCommandOptionBuilder AddStringChoice(string name, string value)
            => this.AddChoice(DiscordApplicationCommandOptionChoice.StringChoice(name, value));
        public DiscordApplicationCommandOptionBuilder AddIntegerChoice(string name, int value)
            => this.AddChoice(DiscordApplicationCommandOptionChoice.IntegerChoice(name, value));
        public DiscordApplicationCommandOptionBuilder AddNumberChoice(string name, double value)
            => this.AddChoice(DiscordApplicationCommandOptionChoice.NumberChoice(name, value));

        public DiscordApplicationCommandOption Build()
        {
            DiscordApplicationCommandOption result = new DiscordApplicationCommandOption(this.Type, this.Name, this.Description);
            result.IsRequired = this.IsRequired;
            if (this.NestedOptions?.Any() == true)
                result.NestedOptions = this.NestedOptions;
            if (this.Choices?.Any() == true)
                result.Choices = this.Choices;
            return result;
        }
    }
}
