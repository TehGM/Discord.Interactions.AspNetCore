using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using TehGM.Discord.Interactions.AspNetCore.Services;
using TehGM.Discord.Interactions.CommandsHandling;

namespace TehGM.Discord.Interactions.AspNetCore.Tests
{
    [TestFixture]
    [Category("Middlewares")]
    public class DiscordInteractionCommandsMiddlewareTests : MiddlewareTestBase
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<DiscordInteractionCommandsMiddleware>();
            services.AddScoped<IDiscordInteractionCommandHandlerProvider, TestInteractionCommandsProvider>();
        }

        protected override void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<DiscordInteractionCommandsMiddleware>();
        }

        [Test]
        public async Task InteractionCommands_KnownCommand_Passes()
        {
            var server = base.Host.GetTestServer();
            var feature = this.GetFeatureForCommand(1234);

            var context = await server.SendAsync(ctx =>
                ctx.Features.Set<IDiscordInteractionReaderFeature>(feature));

            Assert.AreEqual((int)HttpStatusCode.OK, context.Response.StatusCode);
            Assert.AreEqual("application/json", context.Response.Headers["Content-Type"]);
        }

        [Test]
        public async Task InteractionCommands_UnknownCommand_Fails()
        {
            var server = base.Host.GetTestServer();
            var feature = this.GetFeatureForCommand(4321);

            var context = await server.SendAsync(ctx =>
                ctx.Features.Set<IDiscordInteractionReaderFeature>(feature));

            Assert.AreNotEqual((int)HttpStatusCode.OK, context.Response.StatusCode);
        }

        private IDiscordInteractionReaderFeature GetFeatureForCommand(ulong commandID)
        {
            JObject bodyJson = new JObject(
                new JProperty("type", (int)DiscordInteractionType.ApplicationCommand),
                new JProperty("data", new JObject(
                    new JProperty("id", commandID))));
            return new DiscordInteractionReaderFeature(bodyJson.ToString());
        }

        private class TestInteractionCommandsProvider : IDiscordInteractionCommandHandlerProvider
        {
            public IDiscordInteractionCommandHandler GetHandler(ulong commandID)
            {
                if (commandID == 1234)
                    return new TestInteractionCommand();
                return null;
            }
        }

        private class TestInteractionCommand : IDiscordInteractionCommandHandler
        {
            public Task<DiscordInteractionResponse> InvokeAsync(DiscordInteraction message, HttpRequest request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new DiscordInteractionResponseBuilder().Build());
            }
        }
    }
}
