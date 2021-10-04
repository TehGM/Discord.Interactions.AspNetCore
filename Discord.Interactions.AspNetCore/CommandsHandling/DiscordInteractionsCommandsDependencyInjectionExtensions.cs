using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TehGM.Discord.Interactions;
using TehGM.Discord.Interactions.CommandsHandling;
using TehGM.Discord.Interactions.CommandsHandling.Services;
using TehGM.Discord.Interactions.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>Extension methods for registering services for Discord Interactions.</summary>
    public static class DiscordInteractionCommandsDependencyInjectionExtensions
    {
        /// <summary>Adds Discord Interactions services.</summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the servicesto.</param>
        /// <param name="configureOptions">Discord Interactions Options configuration.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddDiscordInteractionCommands(this IServiceCollection services, Action<DiscordInteractionsOptions> configureOptions = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configureOptions != null)
                services.Configure(configureOptions);

            services.TryAddSingleton<IDiscordInteractionCommandsProvider, DiscordInteractionCommandsProvider>();
            services.AddHttpClient<IDiscordHttpClient, DiscordHttpClient>();
            services.TryAddTransient<IDiscordApplicationCommandsClient, DiscordApplicationCommandsClient>();
            services.TryAddTransient<IDiscordApplicationCommandsCreator, DiscordApplicationCommandsCreator>();

            return services;
        }
    }
}
