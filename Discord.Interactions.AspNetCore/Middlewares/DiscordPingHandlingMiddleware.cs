using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace TehGM.Discord.Interactions.AspNetCore
{
    public class DiscordPingHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _log;

        public DiscordPingHandlingMiddleware(RequestDelegate next, ILogger<DiscordPingHandlingMiddleware> log)
        {
            this._next = next;
            this._log = log;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // enable buffering so the body can be read multiple times
            context.Request.EnableBuffering();

            using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true,
                detectEncodingFromByteOrderMarks: true, bufferSize: -1))    // defaults as of .NET 5
            {
                // parse body
                string body = await reader.ReadToEndAsync().ConfigureAwait(false);
                JObject json = JObject.Parse(body);
                // reset stream position so it can be re-read in later middleware
                context.Request.Body.Position = 0;

                // if type == 1, respond with pong
                if (json["type"].Value<int>() == 1)
                {
                    this._log.LogDebug("Discord interaction ping received, returning pong");
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("{ type: 1 }", context.RequestAborted).ConfigureAwait(false);
                    return;
                }
            }

            // otherwise pass on
            this._log.LogTrace("Not a Discord interaction ping message");
            await this._next.Invoke(context);
        }
    }
}
