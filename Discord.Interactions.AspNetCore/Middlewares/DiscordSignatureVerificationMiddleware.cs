using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace TehGM.Discord.Interactions.AspNetCore
{
    public class DiscordSignatureVerificationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptionsMonitor<DiscordInteractionsOptions> _options;
        private readonly ILogger _log;

        public DiscordSignatureVerificationMiddleware(RequestDelegate next, IOptionsMonitor<DiscordInteractionsOptions> options, ILogger<DiscordSignatureVerificationMiddleware> log)
        {
            this._next = next;
            this._options = options;
            this._log = log;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            this._log.LogTrace("Validating Discord's signature");
            if (!context.Request.Headers.TryGetValue("X-Signature-Ed25519", out StringValues signatureValues) ||
                !context.Request.Headers.TryGetValue("X-Signature-Timestamp", out StringValues timestampValues))
            {
                await RespondInvalidSignature().ConfigureAwait(false); ;
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
