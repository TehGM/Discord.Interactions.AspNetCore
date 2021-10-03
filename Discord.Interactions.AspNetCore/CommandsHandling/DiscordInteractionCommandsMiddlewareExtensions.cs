using System;
using Microsoft.Extensions.DependencyInjection;
using TehGM.Discord.Interactions.AspNetCore;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>Extension methods for adding Discord Interactions Commands middlewares.</summary>
    public static class DiscordInteractionCommandssMiddlewareExtensions
    {
        /// <summary>Adds Discord Interaction Commands middlewares to the pipeline.</summary>
        /// <remarks>This method should be called after <see cref="DiscordInteractionsDependencyInjectionExtensions.AddDiscordInteractions(Authentication.AuthenticationBuilder)"/>.</remarks>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseDiscordInteractionCommands(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            return app.UseMiddleware<DiscordInteractionCommandsMiddleware>();
        }
    }
}
