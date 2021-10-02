using System;
using Microsoft.AspNetCore.Authentication;
using TehGM.Discord.Interactions;
using TehGM.Discord.Interactions.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DiscordInteractionsDependencyInjectionExtensions
    {
        public static IServiceCollection AddDiscordInteractions(this IServiceCollection services, Action<DiscordInteractionsOptions> configureOptions = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configureOptions != null)
                services.Configure(configureOptions);

            return services;
        }

        public static AuthenticationBuilder AddDiscordInteractions(this AuthenticationBuilder builder)
            => builder.AddDiscordInteractions(_ => { });

        public static AuthenticationBuilder AddDiscordInteractions(this AuthenticationBuilder builder, Action<DiscordInteractionsAuthenticationOptions> configureAuthentication)
            => builder.AddDiscordInteractions(DiscordInteractionsAuthenticationDefaults.AuthenticationScheme, configureAuthentication);

        public static AuthenticationBuilder AddDiscordInteractions(this AuthenticationBuilder builder, string scheme, Action<DiscordInteractionsAuthenticationOptions> configureAuthentication)
            => builder.AddScheme<DiscordInteractionsAuthenticationOptions, DiscordInteractionsAuthenticationHandler>(scheme, configureAuthentication);
    }
}
