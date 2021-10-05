using System.Collections.Generic;
using System.Reflection;

namespace TehGM.Discord.Interactions.CommandsHandling
{
    /// <summary>A service that can load <see cref="IDiscordInteractionCommandsLoader"/> from assemblies.</summary>
    public interface IDiscordInteractionCommandsLoader
    {
        /// <summary>Loads all <see cref="IDiscordInteractionCommand"/> from an assembly.</summary>
        /// <param name="assembly">Assembly to load commands from.</param>
        /// <returns>Enumerable of all loaded commands.</returns>
        IEnumerable<TypeInfo> LoadFromAssembly(Assembly assembly);
    }
}
