using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Discord.Interactions.CommandsHandling.Services
{
    /// <inheritdoc/>
    public class DiscordInteractionCommandsProvider : IDiscordInteractionCommandsProvider, IDisposable
    {
        private readonly IDictionary<ulong, IDiscordInteractionCommand> _commands;
        private readonly object _lock = new object();

        /// <summary>Initializes a new, empty provider.</summary>
        public DiscordInteractionCommandsProvider()
        {
            this._commands = new Dictionary<ulong, IDiscordInteractionCommand>();
        }

        /// <inheritdoc/>
        public IDiscordInteractionCommand GetCommand(ulong commandID)
        {
            lock (this._lock)
            {
                this._commands.TryGetValue(commandID, out IDiscordInteractionCommand result);
                return result;
            }
        }

        /// <inheritdoc/>
        public void AddCommand(ulong commandID, IDiscordInteractionCommand handler)
        {
            lock (this._lock)
                this._commands.Add(commandID, handler);
        }

        /// <inheritdoc/>
        public bool RemoveCommand(ulong commandID)
        {
            lock (this._lock)
                return this._commands.Remove(commandID);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            lock (this._lock)
                this._commands.Clear();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (this._lock)
            {
                IEnumerable<IDisposable> disposableCommands = this._commands.Values.Where(cmd => cmd is IDisposable).Cast<IDisposable>();
                foreach (IDisposable cmd in disposableCommands)
                    cmd?.Dispose();
                this._commands.Clear();
            }
        }
    }
}
