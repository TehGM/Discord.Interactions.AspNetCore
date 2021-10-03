using System.Collections.Generic;

namespace TehGM.Discord.Interactions.CommandsHandling.Services
{
    class DiscordInteractionCommandsProvider : IDiscordInteractionCommandsProvider
    {
        private readonly IDictionary<ulong, IDiscordInteractionCommand> _commands;

        public DiscordInteractionCommandsProvider()
        {
            this._commands = new Dictionary<ulong, IDiscordInteractionCommand>();
        }

        public IDiscordInteractionCommand GetRegisteredCommand(ulong commandID)
        {
            this._commands.TryGetValue(commandID, out IDiscordInteractionCommand result);
            return result;
        }

        public void RegisterCommand(ulong commandID, IDiscordInteractionCommand handler)
        {
            this._commands.Add(commandID, handler);
        }
    }
}
