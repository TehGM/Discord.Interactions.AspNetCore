using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TehGM.Discord.Interactions.AspNetCore;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>Extension methods for adding Discord Interactions middlewares.</summary>
    public static class DiscordInteractionsMiddlewareExtensions
    {
        /// <summary>Adds Discord Interactions middlewares to the pipeline.</summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseDiscordInteractions(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            DiscordInteractionsMiddlewareOptions options = app.ApplicationServices.GetRequiredService<IOptions<DiscordInteractionsMiddlewareOptions>>().Value;
            return app.UseDiscordInteractions(options);
        }

        /// <summary>Adds Discord Interactions middlewares to the pipeline.</summary>
        /// <param name="options">Instance of options used to configure Discord Interactions middlewares.</param>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseDiscordInteractions(this IApplicationBuilder app, DiscordInteractionsMiddlewareOptions options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            // if routes are null, use on all routes
            if (options.Routes == null)
                app.UseDiscordInteractionsCore(options);
            else
                app.UseWhen(context => options.Routes.Contains(context.Request.Path),
                    builder => builder.UseDiscordInteractionsCore(options));

            return app;
        }

        private static IApplicationBuilder UseDiscordInteractionsCore(this IApplicationBuilder app, DiscordInteractionsMiddlewareOptions options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            app.UseMiddleware<DiscordInteractionReaderMiddleware>();
            app.UseMiddleware<DiscordSignatureVerificationMiddleware>();
            if (options.HandlePings)
                app.UseMiddleware<DiscordPingHandlingMiddleware>(); 
            app.UseMiddleware<DiscordInteractionCommandsMiddleware>();

            return app;
        }
    }
}
