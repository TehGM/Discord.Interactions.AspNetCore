using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TehGM.Discord.Interactions.CommandsHandling.Services
{
    class DiscordInteractionCommandsRegistrar : IHostedService
    {
        private readonly ILogger _log;
        private readonly DiscordInteractionsOptions _options;
        private readonly IServiceProvider _services;
        private readonly IDiscordInteractionCommandsLoader _loader;
        private readonly IDiscordInteractionCommandHandlerFactory _factory;

        public DiscordInteractionCommandsRegistrar(ILogger<DiscordInteractionCommandsRegistrar> log, IOptions<DiscordInteractionsOptions> options, IServiceProvider services,
            IDiscordInteractionCommandsLoader loader, IDiscordInteractionCommandHandlerFactory factory)
        {
            this._log = log;
            this._options = options.Value;
            this._services = services;
            this._loader = loader;
            this._factory = factory;
        }

        /// <inheritdoc/>
        async Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            if (!this._options.RegisterCommands)
            {
                this._log.LogDebug("Registering Discord Application Commands is disabled");
                return;
            }

            this._log.LogInformation("Registering Discord Application Commands");
            this._log.LogTrace("Loading types implementing {Type} from assemblies", nameof(IDiscordInteractionCommand));
            List<TypeInfo> commandTypes = new List<TypeInfo>();
            foreach (Assembly asm in this._options.CommandAssemblies)
                commandTypes.AddRange(this._loader.LoadFromAssembly(asm));

            this._log.LogDebug("{Count} types implementing {Type} interface loaded from assemblies", commandTypes.Count, nameof(IDiscordInteractionCommand));
            if (!commandTypes.Any())
                return;

            // service scope will be used for safe constructor invokation, as well as retrieving HTTP Client
            using IServiceScope scope = this._services.CreateScope();

            // perform registration
            await this.RegisterGlobalCommandsAsync(scope.ServiceProvider, commandTypes, cancellationToken).ConfigureAwait(false);
            await this.RegisterGuildCommandsAsync(scope.ServiceProvider, commandTypes, cancellationToken).ConfigureAwait(false);
        }

        private Task RegisterGlobalCommandsAsync(IServiceProvider services, IEnumerable<TypeInfo> handlerTypes, CancellationToken cancellationToken)
        {
            // filter out guild commands
            handlerTypes = handlerTypes.Where(type =>
                type.GetCustomAttribute<GuildInteractionCommandAttribute>() == null);
            if (handlerTypes?.Any() != true)
                return Task.CompletedTask;

            this._log.LogDebug("Registering global Discord Application commands");
            return this.RegisterCommandsInternalAsync(services, handlerTypes, null, cancellationToken);
        }

        private async Task RegisterGuildCommandsAsync(IServiceProvider services, IEnumerable<TypeInfo> handlerTypes, CancellationToken cancellationToken)
        {
            // filter out commands that don't have guild command attribute
            handlerTypes = handlerTypes.Where(type => 
                type.GetCustomAttribute<GuildInteractionCommandAttribute>() != null);
            if (handlerTypes?.Any() != true)
                return;

            // select all guild IDs
            IEnumerable<ulong> guildIDs = handlerTypes.SelectMany(type => 
                type.GetCustomAttribute<GuildInteractionCommandAttribute>().GuildIDs).Distinct();
            if (guildIDs?.Any() != true)
                return;

            foreach (ulong gid in guildIDs)
            {
                using IDisposable logScope = this._log.BeginScope(new Dictionary<string, object> { { "GuildID", gid } });
                this._log.LogDebug("Registering Discord Application commands for guild {GuildID}", gid);

                // get commands only for given guild ID
                IEnumerable<TypeInfo> guildCommandTypes = handlerTypes.Where(type => type
                    .GetCustomAttribute<GuildInteractionCommandAttribute>().GuildIDs.Contains(gid));

                // register them
                await this.RegisterCommandsInternalAsync(services, guildCommandTypes, gid, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task RegisterCommandsInternalAsync(IServiceProvider services, IEnumerable<TypeInfo> handlerTypes, ulong? guildID, CancellationToken cancellationToken)
        {
            if (handlerTypes?.Any() != true)
                return;

            // map commands based on name - will be needed to get IDs
            // do this by building commands while at it, I guess
            List<DiscordApplicationCommand> builtCommands = new List<DiscordApplicationCommand>(handlerTypes.Count());
            Dictionary<CommandKey, ServiceDescriptor> commandNames = new Dictionary<CommandKey, ServiceDescriptor>(handlerTypes.Count());
            foreach (TypeInfo type in handlerTypes)
            {
                // create descriptor
                ServiceLifetime lifetime = type.GetCustomAttribute<InteractionCommandLifetimeAttribute>()?.Lifetime
                    ?? InteractionCommandLifetimeAttribute.DefaultLifetime;
                ServiceDescriptor descriptor = new ServiceDescriptor(type, type, lifetime);

                // build command
                DiscordApplicationCommand builtCmd = this.BuildApplicationCommand(descriptor, services);
                builtCommands.Add(builtCmd);

                // store result
                commandNames.Add(CommandKey.FromCommand(builtCmd), descriptor);
                this._log.LogTrace("Built Discord Application command: {Name}", builtCmd.Name);
            }

            try
            {
                // register commands with the client
                this._log.LogTrace("Sending Discord request to register {Count} commands", builtCommands.Count);
                IEnumerable<DiscordApplicationCommand> results;
                IDiscordApplicationCommandsClient client = services.GetRequiredService<IDiscordApplicationCommandsClient>();
                if (guildID != null)
                    results = await client.RegisterGuildCommandsAsync(guildID.Value, builtCommands, cancellationToken).ConfigureAwait(false);
                else
                    results = await client.RegisterGlobalCommandsAsync(builtCommands, cancellationToken).ConfigureAwait(false);

                // for each result, find mapped commands via name, and save descriptor
                foreach (DiscordApplicationCommand registeredCmd in results)
                {
                    ServiceDescriptor descriptor = commandNames[CommandKey.FromCommand(registeredCmd)];
                    this._factory.AddDescriptor(registeredCmd.ID, descriptor);
                    this._log.LogDebug("Registered command {Name} with ID {ID}", registeredCmd.Name, registeredCmd.ID);
                }
            }
            catch (Exception ex) when (this.LogAsError(ex, GetExceptionMessage(ex))) { }

            string GetExceptionMessage(Exception ex)
            {
                string exMsg = "Failed registering Discord Application commands";
                if (guildID != null)
                    exMsg += " for guild ID {GuildID}";
                return exMsg;
            }
        }

        private DiscordApplicationCommand BuildApplicationCommand(ServiceDescriptor descriptor, IServiceProvider scopedServices)
        {
            // buildable commands need to be initialized briefly
            if (typeof(IBuildableDiscordInteractionCommand).IsAssignableFrom(descriptor.ServiceType))
            {
                IBuildableDiscordInteractionCommand handler = this._factory.CreateCommand(descriptor, scopedServices) as IBuildableDiscordInteractionCommand;
                DiscordApplicationCommand result = handler.Build();
                try { (handler as IDisposable)?.Dispose(); } catch { }
                return result;
            }
            // non-buildable = build from the attribute
            else
            {
                InteractionCommandAttribute commandAttribute = descriptor.ImplementationType.GetCustomAttribute<InteractionCommandAttribute>(inherit: true);
                if (commandAttribute == null)
                    throw new InvalidOperationException($"Command handler {descriptor.ImplementationType.GetType().FullName} cannot be built - {nameof(InteractionCommandAttribute)} not present.");
                return new DiscordApplicationCommand(commandAttribute.CommandType, commandAttribute.Name, commandAttribute.Description, true);
            }
        }

        private bool LogAsError(Exception exception, string message, params object[] args)
        {
            this._log.LogError(exception, message, args);
            return true;
        }

        /// <inheritdoc/>
        Task IHostedService.StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        /// <summary>Compound key for Discord Application Command.</summary>
        /// <remarks>Until commands are actually registered with Discord Servers, their ID will be unknown. Name alone isn't good enough either, as commands with 
        /// same name can be of different type. This struct combines name and type into a single key that can be used in dictionaries during registration lookups.</remarks>
        public struct CommandKey : IEquatable<CommandKey>
        {
            public string Name { get; }
            public DiscordApplicationCommandType Type { get; }

            public CommandKey(string name, DiscordApplicationCommandType type)
            {
                this.Name = name;
                this.Type = type;
            }

            public bool Equals(CommandKey other)
                => this.Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) && this.Type == other.Type;

            public override bool Equals(object obj)
            {
                if (obj is CommandKey key)
                    return this.Equals(key);
                return false;
            }

            public override int GetHashCode()
                => HashCode.Combine(this.Name.GetHashCode(StringComparison.OrdinalIgnoreCase), Type);

            public static bool operator ==(CommandKey left, CommandKey right)
                => left.Equals(right);

            public static bool operator !=(CommandKey left, CommandKey right)
                => !(left == right);

            public static CommandKey FromCommand(DiscordApplicationCommand command)
                => new CommandKey(command.Name, command.Type);
        }
    }
}
