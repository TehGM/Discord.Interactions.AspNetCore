using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TehGM.Discord.Interactions;
using TehGM.Discord.Interactions.AspNetCore.Authentication;
using TehGM.Discord.Interactions.CommandsHandling;
using TehGM.Discord.Interactions.CommandsHandling.Services;
using TehGM.Discord.Interactions.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>Extension methods for registering services for Discord Interactions.</summary>
    public static class DiscordInteractionsDependencyInjectionExtensions
    {
        /// <summary>Adds Discord Interactions services.</summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the servicesto.</param>
        /// <param name="configureOptions">Discord Interactions Options configuration.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddDiscordInteractions(this IServiceCollection services, Action<DiscordInteractionsOptions> configureOptions = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configureOptions != null)
                services.Configure(configureOptions);

            services.TryAddSingleton<IDiscordInteractionCommandsProvider, DiscordInteractionCommandsProvider>();
            services.AddHttpClient<IDiscordHttpClient, DiscordHttpClient>();
            services.TryAddTransient<IDiscordApplicationCommandsClient, DiscordApplicationCommandsClient>();
            services.TryAddTransient<IDiscordApplicationCommandsCreator, DiscordApplicationCommandsCreator>();
            services.TryAddTransient<IDiscordInteractionCommandsLoader, DiscordInteractionCommandsLoader>();
            services.AddHostedService<DiscordInteractionCommandsRegistrar>();

            return services;
        }

        /// <summary>Adds Discord Interactions authentication to <see cref="AuthenticationBuilder"/> using the default scheme. 
        /// The default scheme is specified by <see cref="DiscordInteractionsAuthenticationDefaults.AuthenticationScheme"/>.</summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <returns>A reference to builder after the operation has completed.</returns>
        public static AuthenticationBuilder AddDiscordInteractions(this AuthenticationBuilder builder)
            => builder.AddDiscordInteractions(_ => { });

        /// <summary>Adds Discord Interactions authentication to <see cref="AuthenticationBuilder"/> using the default scheme. 
        /// The default scheme is specified by <see cref="DiscordInteractionsAuthenticationDefaults.AuthenticationScheme"/>.</summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="configureAuthentication">A delegate to configure <see cref="DiscordInteractionsAuthenticationOptions"/>.</param>
        /// <returns>A reference to builder after the operation has completed.</returns>
        public static AuthenticationBuilder AddDiscordInteractions(this AuthenticationBuilder builder, Action<DiscordInteractionsAuthenticationOptions> configureAuthentication)
            => builder.AddDiscordInteractions(DiscordInteractionsAuthenticationDefaults.AuthenticationScheme, configureAuthentication);

        /// <summary>Adds Discord Interactions authentication to <see cref="AuthenticationBuilder"/> using the specified scheme.</summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="scheme">The authentication scheme.</param>
        /// <param name="configureAuthentication">A delegate to configure <see cref="DiscordInteractionsAuthenticationOptions"/>.</param>
        /// <returns>A reference to builder after the operation has completed.</returns>
        public static AuthenticationBuilder AddDiscordInteractions(this AuthenticationBuilder builder, string scheme, Action<DiscordInteractionsAuthenticationOptions> configureAuthentication)
            => builder.AddScheme<DiscordInteractionsAuthenticationOptions, DiscordInteractionsAuthenticationHandler>(scheme, configureAuthentication);
    }
}
