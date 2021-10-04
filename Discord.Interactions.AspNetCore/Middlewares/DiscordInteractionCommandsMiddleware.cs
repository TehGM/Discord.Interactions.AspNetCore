﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TehGM.Discord.Interactions.CommandsHandling;

namespace TehGM.Discord.Interactions.AspNetCore
{
    /// <summary>A middleware that invokes registered <see cref="IDiscordInteractionCommand"/>.</summary>
    /// <remarks><para>When an interaction is received, this middleware will check registered command handlers for one that can handle the interaction.
    /// If the handler was found, the command will be invoked, and no further middleware or controller will run.</para>
    /// <para>If no command with matching ID is found, the request will be passed further the middleware pipeline.</para></remarks>
    public class DiscordInteractionCommandsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _log;
        private readonly IDiscordInteractionCommandsProvider _commands;

        /// <summary>Creates an instance of the middleware.</summary>
        /// <param name="next">Delegate to the next middleware.</param>
        /// <param name="log">Logger this middleware will use to log messages to.</param>
        /// <param name="commands">Provider of registered Interaction Commands.</param>
        public DiscordInteractionCommandsMiddleware(RequestDelegate next, ILogger<DiscordInteractionCommandsMiddleware> log, IDiscordInteractionCommandsProvider commands)
        {
            this._next = next;
            this._log = log;
            this._commands = commands;
        }

        /// <summary>Invokes the middleware for given request context.</summary>
        /// <param name="context">The request context.</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // check command ID. If it cannot be retrieved, pass
            IDiscordInteractionReaderFeature feature = context.Features.Get<IDiscordInteractionReaderFeature>();
            ulong? commandID = feature?.InteractionJson?["data"]?["id"]?.Value<ulong>();
            if (commandID == null)
            {
                await this._next.Invoke(context).ConfigureAwait(false);
                return;
            }
            // try to get command handler
            // if cannot, then also pass
            IDiscordInteractionCommand cmd = this._commands.GetRegisteredCommand(commandID.Value);
            if (cmd == null)
            {
                await this._next.Invoke(context).ConfigureAwait(false);
                return;
            }

            // command was found, so invoke it, and return the message
            this._log.LogDebug("Invoking command {ID}", commandID.Value);
            DiscordInteractionResponse response = await cmd.InvokeAsync(feature.Interaction, context.Request, context.RequestServices, context.RequestAborted).ConfigureAwait(false);
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync(JObject.FromObject(response).ToString(Formatting.None), context.RequestAborted).ConfigureAwait(false);
        }
    }
}