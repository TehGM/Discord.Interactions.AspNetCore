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

        public IDiscordInteractionCommand GetCommand(ulong commandID)
        {
            this._commands.TryGetValue(commandID, out IDiscordInteractionCommand result);
            return result;
        }

        public void AddCommand(ulong commandID, IDiscordInteractionCommand handler)
        {
            this._commands.Add(commandID, handler);
        }
    }
}
