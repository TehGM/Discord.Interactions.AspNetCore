using System;
using System.Reflection;

namespace TehGM.Discord.Interactions.CommandsHandling.Services
{
    /// <inheritdoc/>
    public class DiscordApplicationCommandsCreator : IDiscordApplicationCommandsCreator
    {
        private readonly IServiceProvider _services;

        /// <summary>Creates an instance of the class.</summary>
        /// <param name="services">Service provider that will be passed to any <see cref="IBuildableDiscordInteractionCommand"/>.</param>
        public DiscordApplicationCommandsCreator(IServiceProvider services)
        {
            this._services = services;
        }

        /// <inheritdoc/>
        public DiscordApplicationCommand Create(IDiscordInteractionCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (command is IBuildableDiscordInteractionCommand buildable)
                return buildable.Build(this._services);

            Type type = command.GetType();
            InteractionCommandAttribute cmdAttribute = type.GetCustomAttribute<InteractionCommandAttribute>(true);
            if (cmdAttribute == null)
                throw new ArgumentException($"Interaction Command type {type.FullName} has no {nameof(InteractionCommandAttribute)} specified.", nameof(command));

            return new DiscordApplicationCommand(cmdAttribute.CommandType, cmdAttribute.Name, cmdAttribute.Description, true);
        }
    }
}
