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

namespace TehGM.Discord.Interactions.CommandsHandling.Registration.Services
{
    /// <summary>A service that handles registration of new Discord Application Commands.</summary>
    public class DiscordInteractionCommandsRegistrar : IDiscordInteractionCommandsRegistrar, IHostedService
    {
        private readonly ILogger _log;
        private readonly DiscordInteractionsOptions _options;
        private readonly IServiceProvider _services;
        private readonly IDiscordInteractionCommandsLoader _loader;
        private readonly IDiscordInteractionCommandHandlerFactory _factory;
        private readonly IDiscordInteractionCommandBuilder _builder;

        /// <summary>Creates a new instance of the service.</summary>
        /// <param name="log">Used for logging registration events.</param>
        /// <param name="options">Options for registration.</param>
        /// <param name="services">Service provider that can be used when injecting dependencies.</param>
        /// <param name="loader">Service loading command handlers from assemblies.</param>
        /// <param name="factory">Factory for command handlers.</param>
        /// <param name="builder">Service that builds commands.</param>
        public DiscordInteractionCommandsRegistrar(ILogger<DiscordInteractionCommandsRegistrar> log, IOptions<DiscordInteractionsOptions> options, IServiceProvider services,
            IDiscordInteractionCommandsLoader loader, IDiscordInteractionCommandHandlerFactory factory, IDiscordInteractionCommandBuilder builder)
        {
            this._log = log;
            this._options = options.Value;
            this._services = services;
            this._loader = loader;
            this._factory = factory;
            this._builder = builder;
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
            this._log.LogTrace("Loading types implementing {Type} from assemblies", nameof(IDiscordInteractionCommandHandler));
            List<TypeInfo> commandTypes = new List<TypeInfo>();
            foreach (Assembly asm in this._options.CommandAssemblies)
                commandTypes.AddRange(this._loader.LoadFromAssembly(asm));

            this._log.LogDebug("{Count} types implementing {Type} interface loaded from assemblies", commandTypes.Count, nameof(IDiscordInteractionCommandHandler));
            if (!commandTypes.Any())
                return;

            // service scope will be used for HTTP Client only
            using IServiceScope scope = this._services.CreateScope();
            IDiscordApplicationCommandsClient client = scope.ServiceProvider.GetRequiredService<IDiscordApplicationCommandsClient>();

            // perform registration
            await this.RegisterGlobalCommandsAsync(client, commandTypes, cancellationToken).ConfigureAwait(false);
            await this.RegisterGuildCommandsAsync(client, commandTypes, cancellationToken).ConfigureAwait(false);
        }

        private Task RegisterGlobalCommandsAsync(IDiscordApplicationCommandsClient client, IEnumerable<TypeInfo> handlerTypes, CancellationToken cancellationToken)
        {
            // filter out guild commands
            handlerTypes = handlerTypes.Where(type =>
                type.GetCustomAttribute<GuildInteractionCommandAttribute>() == null);
            if (handlerTypes?.Any() != true)
                return Task.CompletedTask;

            this._log.LogDebug("Registering global Discord Application commands");
            return this.RegisterCommandsInternalAsync(client, handlerTypes, null, cancellationToken);
        }

        private async Task RegisterGuildCommandsAsync(IDiscordApplicationCommandsClient client, IEnumerable<TypeInfo> handlerTypes, CancellationToken cancellationToken)
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
                await this.RegisterCommandsInternalAsync(client, guildCommandTypes, gid, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task RegisterCommandsInternalAsync(IDiscordApplicationCommandsClient client, IEnumerable<TypeInfo> handlerTypes, ulong? guildID, CancellationToken cancellationToken)
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
                DiscordApplicationCommand builtCmd = await this._builder.BuildAsync(type, cancellationToken).ConfigureAwait(false);
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
            catch (Exception ex) when (LogRegistrationException(ex)) { }

            bool LogRegistrationException(Exception exception)
            {
                if (guildID == null)
                    this._log.LogError(exception, "Failed registering Discord Application commands");
                else
                    this._log.LogError(exception, "Failed registering Discord Application commands for guild ID {GuildID}", guildID);
                return true;
            }
        }

        /// <inheritdoc/>
        Task IHostedService.StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        /// <summary>Compound key for Discord Application Command.</summary>
        /// <remarks>Until commands are actually registered with Discord Servers, their ID will be unknown. Name alone isn't good enough either, as commands with 
        /// same name can be of different type. This struct combines name and type into a single key that can be used in dictionaries during registration lookups.</remarks>
        private struct CommandKey : IEquatable<CommandKey>
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
