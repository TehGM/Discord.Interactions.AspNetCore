using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Discord.Interactions.CommandsHandling.Services
{
    class DiscordInteractionCommandsProvider : IDiscordInteractionCommandsProvider, IDisposable
    {
        private readonly IDictionary<ulong, IDiscordInteractionCommand> _commands;
        private readonly object _lock = new object();

        public DiscordInteractionCommandsProvider()
        {
            this._commands = new Dictionary<ulong, IDiscordInteractionCommand>();
        }

        public IDiscordInteractionCommand GetCommand(ulong commandID)
        {
            lock (_lock)
            {
                this._commands.TryGetValue(commandID, out IDiscordInteractionCommand result);
                return result;
            }
        }

        public void AddCommand(ulong commandID, IDiscordInteractionCommand handler)
        {
            lock (_lock)
            {
                this._commands.Add(commandID, handler);
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                IEnumerable<IDisposable> disposableCommands = this._commands.Values.Where(cmd => cmd is IDisposable).Cast<IDisposable>();
                foreach (IDisposable cmd in disposableCommands)
                    cmd?.Dispose();
                this._commands.Clear();
            }
        }
    }
}
