using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace TehGM.Discord.Interactions.CommandsHandling.Services
{
    class DiscordInteractionCommandsLoader : IDiscordInteractionCommandsLoader
    {
        private readonly ILogger _log;

        public DiscordInteractionCommandsLoader(ILogger<DiscordInteractionCommandsLoader> log)
        {
            this._log = log;
        }

        public IEnumerable<IDiscordInteractionCommand> LoadFromAssembly(Assembly assembly)
        {
            _log?.LogTrace("Loading assembly {Name}", assembly.FullName);
            IEnumerable<TypeInfo> types = assembly.DefinedTypes.Where(t => !t.IsAbstract && !t.ContainsGenericParameters
                && !Attribute.IsDefined(t, typeof(CompilerGeneratedAttribute)) && typeof(IDiscordInteractionCommand).IsAssignableFrom(t));
            if (!types.Any())
            {
                _log?.LogWarning("Cannot load commands from assembly {Name} - no non-static non-abstract non-generic classes implementing {Type} interface", assembly.FullName, nameof(IDiscordInteractionCommand));
                return Enumerable.Empty<IDiscordInteractionCommand>();
            }

            return types.Select(t => this.LoadFromType(t));
        }

        public IDiscordInteractionCommand LoadFromType(TypeInfo type)
        {
            if (!typeof(IDiscordInteractionCommand).IsAssignableFrom(type))
                throw new ArgumentException($"Type {type.FullName} does not implement {nameof(IDiscordInteractionCommand)} interface.", nameof(type));

            _log?.LogTrace("Loading type {Type}", type.FullName);
            return (IDiscordInteractionCommand)Activator.CreateInstance(type, true);
        }
    }
}
