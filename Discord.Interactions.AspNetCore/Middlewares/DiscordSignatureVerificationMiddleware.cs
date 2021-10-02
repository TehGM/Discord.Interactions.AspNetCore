using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace TehGM.Discord.Interactions.AspNetCore
{
    /// <summary>A middleware that automatically validates Discord's signature.</summary>
    /// <remarks><para>Discord sends all interactions with a signature, and requires that signature is validated to prevent bad actors interacting with your endpoints. 
    /// Additionally Discord will periodically send an interaction with invalid signature to validate that your application does the signature validation.</para>
    /// <para>This middleware handles signature validation, and automatically responds with status code 401 if validation has failed.</para>
    /// <para>This middleware is short-circuiting. This means that if signature validation has failed, no further middleware or controllers will receive the message.</para></remarks>
    /// <seealso href="https://discord.com/developers/docs/interactions/receiving-and-responding#security-and-authorization"/>
    public class DiscordSignatureVerificationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptionsMonitor<DiscordInteractionsOptions> _options;
        private readonly ILogger _log;

        /// <summary>Creates an instance of the middleware.</summary>
        /// <param name="next">Delegate to the next middleware.</param>
        /// <param name="options">Options with <see cref="DiscordInteractionsOptions.PublicKey"/> used to validate the signature.</param>
        /// <param name="log">Logger this middleware will use to log messages to.</param>
        public DiscordSignatureVerificationMiddleware(RequestDelegate next, IOptionsMonitor<DiscordInteractionsOptions> options, ILogger<DiscordSignatureVerificationMiddleware> log)
        {
            this._next = next;
            this._options = options;
            this._log = log;
        }

        /// <summary>Invokes the middleware for given request context.</summary>
        /// <param name="context">The request context.</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            this._log.LogTrace("Validating Discord's signature");
            if (!context.Request.Headers.TryGetValue("X-Signature-Ed25519", out StringValues signatureValues) ||
                !context.Request.Headers.TryGetValue("X-Signature-Timestamp", out StringValues timestampValues))
            {
                await RespondInvalidSignature().ConfigureAwait(false);
                return;
            }

            // enable buffering so the body can be read multiple times
            context.Request.EnableBuffering();

            byte[] key = Sodium.Utilities.HexToBinary(this._options.CurrentValue.PublicKey);
            byte[] signature = Sodium.Utilities.HexToBinary(signatureValues.ToString());
            byte[] message;
            using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true,
                detectEncodingFromByteOrderMarks: true, bufferSize: -1))    // defaults as of .NET 5
            {
                string timestamp = timestampValues.ToString();
                string body = await reader.ReadToEndAsync().ConfigureAwait(false);
                message = Encoding.UTF8.GetBytes(timestamp + body);

                // reset stream position so it can be re-read in later middleware
                context.Request.Body.Position = 0;
            }

            if (!Sodium.PublicKeyAuth.VerifyDetached(signature, message, key))
            {
                await RespondInvalidSignature().ConfigureAwait(false);
                return;
            }
            else
            {
                this._log.LogTrace("Discord signature valid");
                await this._next.Invoke(context);
            }

            Task RespondInvalidSignature()
            {
                this._log.LogDebug("Discord signature not valid, returning 401 Unauthorized");
                context.Response.StatusCode = 401;
                return context.Response.WriteAsync("Invalid signature");
            }
        }
    }
}
