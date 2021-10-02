using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Discord.Interactions
{
    public class DiscordApplicationCommandBuilder
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DiscordApplicationCommandType Type { get; set; }
        public bool EnabledByDefault { get; set; }
        public ICollection<DiscordApplicationCommandOption> Options { get; set; }

        public DiscordApplicationCommandBuilder(DiscordApplicationCommand existingCommand)
        {
            DiscordApplicationCommand clone = (DiscordApplicationCommand)existingCommand.Clone();
            this.Name = clone.Name;
            this.Description = clone.Description;
            this.Type = clone.Type;
            this.EnabledByDefault = clone.EnabledByDefault ?? true;
            this.Options = clone.Options;
        }

        public DiscordApplicationCommandBuilder(DiscordApplicationCommandType type, string name, string description)
        {
            this.Type = type;
            this.Name = name;
            this.Description = description;
            this.EnabledByDefault = true;
        }

        public DiscordApplicationCommandBuilder(string name) : this(DiscordApplicationCommandType.ChatInput, name, null) { }
        public DiscordApplicationCommandBuilder() : this((string)null) { }

        private DiscordApplicationCommandBuilder Modify(Action<DiscordApplicationCommandBuilder> builder)
        {
            builder.Invoke(this);
            return this;
        }

        public static DiscordApplicationCommandBuilder CreateSlashCommand(string name, string description)
            => new DiscordApplicationCommandBuilder(DiscordApplicationCommandType.ChatInput, name, description);
        public static DiscordApplicationCommandBuilder CreateUserCommand(string name)
            => new DiscordApplicationCommandBuilder(DiscordApplicationCommandType.User, name, null);
        public static DiscordApplicationCommandBuilder CreateMessageCommand(string name)
            => new DiscordApplicationCommandBuilder(DiscordApplicationCommandType.Message, name, null);

        public DiscordApplicationCommandBuilder WithName(string name)
            => this.Modify(cmd => cmd.Name = name);
        public DiscordApplicationCommandBuilder WithDescription(string description)
            => this.Modify(cmd => cmd.Description = description);
        public DiscordApplicationCommandBuilder WithDefaultEnabledState(bool enableByDefault)
            => this.Modify(cmd => cmd.EnabledByDefault = enableByDefault);
        public DiscordApplicationCommandBuilder WithType(DiscordApplicationCommandType type)
            => this.Modify(cmd => cmd.Type = type);

        public DiscordApplicationCommandBuilder AddOption(DiscordApplicationCommandOption option)
        {
            if (this.Options == null)
                this.Options = new List<DiscordApplicationCommandOption>();
            this.Options.Add(option);
            return this;
        }
        public DiscordApplicationCommandBuilder AddOption(Action<DiscordApplicationCommandOptionBuilder> configure)
        {
            DiscordApplicationCommandOptionBuilder builder = new DiscordApplicationCommandOptionBuilder();
            configure.Invoke(builder);
            return this.AddOption(builder.Build());
        }

        public DiscordApplicationCommand Build()
        {
            DiscordApplicationCommand result = new DiscordApplicationCommand(this.Type, this.Name, this.Description, this.EnabledByDefault);
            if (this.Options?.Any() == true)
                result.Options = this.Options;
            return result;
        }
    }
}
