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
        private readonly IDiscordApplicationCommandsCreator _creator;
        private readonly IDiscordInteractionCommandsProvider _provider;

        public DiscordInteractionCommandsRegistrar(ILogger<DiscordInteractionCommandsRegistrar> log, IOptions<DiscordInteractionsOptions> options, IServiceProvider services,
            IDiscordInteractionCommandsLoader loader, IDiscordApplicationCommandsCreator creator, IDiscordInteractionCommandsProvider provider)
        {
            this._log = log;
            this._options = options.Value;
            this._services = services;
            this._loader = loader;
            this._creator = creator;
            this._provider = provider;
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
            List<IDiscordInteractionCommand> commands = new List<IDiscordInteractionCommand>();
            foreach (Assembly asm in this._options.CommandAssemblies)
                commands.AddRange(this._loader.LoadFromAssembly(asm));

            this._log.LogDebug("{Count} instances of {Type} loaded from assemblies", commands.Count, nameof(IDiscordInteractionCommand));
            if (!commands.Any())
                return;

            // it is still unclear to me how injecting transient HTTP client
            // into hosted service will work (as in, if it'll have any implications)
            // for that reason, use IServiceProvider directly to resolve the client here
            // this way it'll go out of scope when this method ends
            this._log.LogTrace("Resolving IDiscordApplicationCommandsClient");
            using IServiceScope scope = this._services.CreateScope();
            IDiscordApplicationCommandsClient client = scope.ServiceProvider.GetRequiredService<IDiscordApplicationCommandsClient>();

            // perform registration
            await this.RegisterGlobalCommandsAsync(client, commands, cancellationToken).ConfigureAwait(false);
            await this.RegisterGuildCommandsAsync(client, commands, cancellationToken).ConfigureAwait(false);
        }

        private Task RegisterGlobalCommandsAsync(IDiscordApplicationCommandsClient client, IEnumerable<IDiscordInteractionCommand> commands, CancellationToken cancellationToken)
        {
            // filter out guild commands
            commands = commands.Where(cmd =>
                cmd.GetType().GetCustomAttribute<GuildInteractionCommandAttribute>() == null);
            if (commands?.Any() != true)
                return Task.CompletedTask;

            this._log.LogDebug("Registering global Discord Application commands");
            return this.RegisterCommandsInternalAsync(client, commands, null, cancellationToken);
        }

        private async Task RegisterGuildCommandsAsync(IDiscordApplicationCommandsClient client, IEnumerable<IDiscordInteractionCommand> commands, CancellationToken cancellationToken)
        {
            // filter out commands that don't have guild command attribute
            commands = commands.Where(cmd => cmd.GetType().GetCustomAttribute<GuildInteractionCommandAttribute>() != null);
            if (commands?.Any() != true)
                return;

            // select all guild IDs
            IEnumerable<ulong> guildIDs = commands.SelectMany(cmd => cmd.GetType().GetCustomAttribute<GuildInteractionCommandAttribute>().GuildIDs).Distinct();
            if (guildIDs?.Any() != true)
                return;

            foreach (ulong gid in guildIDs)
            {
                using IDisposable logScope = this._log.BeginScope(new Dictionary<string, object> { { "GuildID", gid } });
                this._log.LogDebug("Registering Discord Application commands for guild {GuildID}", gid);

                // get commands only for given guild ID
                IEnumerable<IDiscordInteractionCommand> guildCommands = commands.Where(cmd => cmd.GetType().GetCustomAttribute<GuildInteractionCommandAttribute>().GuildIDs.Contains(gid));

                // register them
                await this.RegisterCommandsInternalAsync(client, guildCommands, gid, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task RegisterCommandsInternalAsync(IDiscordApplicationCommandsClient client, IEnumerable<IDiscordInteractionCommand> commands, ulong? guildID, CancellationToken cancellationToken)
        {
            if (commands?.Any() != true)
                return;

            // map commands based on name - will be needed to get IDs
            // do this by building commands while at it, I guess
            List<DiscordApplicationCommand> builtCommands = new List<DiscordApplicationCommand>(commands.Count());
            Dictionary<CommandKey, IDiscordInteractionCommand> commandNames = new Dictionary<CommandKey, IDiscordInteractionCommand>(commands.Count());
            foreach (IDiscordInteractionCommand cmd in commands)
            {
                DiscordApplicationCommand builtCmd = this._creator.Create(cmd);
                builtCommands.Add(builtCmd);
                commandNames.Add(CommandKey.FromCommand(builtCmd), cmd);
                this._log.LogTrace("Built Discord Application command: {Name}", builtCmd.Name);
            }

            try
            {
                this._log.LogTrace("Sending Discord request to register {Count} commands", builtCommands.Count);
                IEnumerable<DiscordApplicationCommand> results;
                if (guildID != null)
                    results = await client.RegisterGuildCommandsAsync(guildID.Value, builtCommands, cancellationToken).ConfigureAwait(false);
                else
                    results = await client.RegisterGlobalCommandsAsync(builtCommands, cancellationToken).ConfigureAwait(false);

                // for each result, find mapped commands via name, and cache it
                foreach (DiscordApplicationCommand registeredCmd in results)
                {
                    IDiscordInteractionCommand cmd = commandNames[CommandKey.FromCommand(registeredCmd)];
                    this._provider.RegisterCommand(registeredCmd.ID, cmd);
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
