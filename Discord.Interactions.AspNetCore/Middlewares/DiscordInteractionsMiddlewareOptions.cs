using System;
using System.Collections.Generic;
using TehGM.Discord.Interactions.AspNetCore;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>Represents options of how Discord Interactions middlewares should behave.</summary>
    /// <remarks>These options are used only during middleware registration.</remarks>
    public class DiscordInteractionsMiddlewareOptions
    {
        /// <summary>The routes that middlewares should be enabled on. All routes must begin with leading '/' in order to be recognized.</summary>
        /// <remarks><para>`/api/discord/interactions` route is added by default. Clear or recreate this collection if you wish to remove the default route.</para>
        /// <para>Multiple routes can be added. Middlewares will run on all added routes.</para>
        /// <para>To make middlewares run on all routes, set this value to null.</para>
        /// <para>Routes are case insensitive by default. To change this, recreate the collection object.</para>
        /// <para>Please note that <see cref="DiscordSignatureVerificationMiddleware"/> will be ran on all provided routes. Do not add routes that need to be used for non-interactions operations.</para></remarks>
        /// <seealso href="https://discord.com/developers/docs/interactions/receiving-and-responding#security-and-authorization"/>
        /// <seealso cref="DiscordSignatureVerificationMiddleware"/>
        public ICollection<string> Routes { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "/api/discord/interactions" };
        /// <summary>Whether middleware for ping message handling should be registered. Defaults to true.</summary>
        /// <remarks>This value directly determines if <see cref="DiscordPingHandlingMiddleware"/> should be added to pipeline when calling <see cref="DiscordInteractionsMiddlewareExtensions.UseDiscordInteractions(IApplicationBuilder)"/>.
        /// After that method is called, changing this property will have no effect.</remarks>
        /// <seealso href="https://discord.com/developers/docs/interactions/receiving-and-responding#receiving-an-interaction"/>
        public bool HandlePings { get; set; } = true;
    }
}
