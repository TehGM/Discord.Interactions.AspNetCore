using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using TehGM.Discord.Interactions.CommandsHandling;

namespace TehGM.Discord.Interactions.AspNetCore.Tests
{
    [TestFixture]
    [Category("Middlewares")]
    public class DiscordInteractionCommandsMiddlewareTests : MiddlewareTestBase
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDiscordInteractionCommandsProvider, TestInteractionCommandsProvider>();
        }

        protected override void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<DiscordInteractionReaderMiddleware>();
            app.UseMiddleware<DiscordInteractionCommandsMiddleware>();
        }

        [Test]
        public async Task InteractionCommands_KnownCommand_Passes()
        {
            HttpClient client = base.Host.GetTestClient();
            JObject bodyJson = new JObject(
                new JProperty("type", DiscordInteractionType.ApplicationCommand),
                new JProperty("data", new JObject(
                    new JProperty("id", 1234))));

            HttpContent body = new StringContent(bodyJson.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/discord/interactions", body);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task InteractionCommands_UnknownCommand_Fails()
        {
            HttpClient client = base.Host.GetTestClient();
            JObject bodyJson = new JObject(
                new JProperty("type", DiscordInteractionType.ApplicationCommand),
                new JProperty("data", new JObject(
                    new JProperty("id", 4321))));

            HttpContent body = new StringContent(bodyJson.ToString(), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/discord/interactions", body);

            Assert.AreNotEqual(HttpStatusCode.OK, response.StatusCode);
        }

        private class TestInteractionCommandsProvider : IDiscordInteractionCommandsProvider
        {
            public IDiscordInteractionCommand GetCommand(ulong commandID)
            {
                if (commandID == 1234)
                    return new TestInteractionCommand();
                return null;
            }

            public void AddCommand(ulong commandID, IDiscordInteractionCommand handler)
            {
                throw new NotImplementedException();
            }
        }

        private class TestInteractionCommand : IDiscordInteractionCommand
        {
            public Task<DiscordInteractionResponse> InvokeAsync(DiscordInteraction message, HttpRequest request, IServiceProvider services, CancellationToken cancellationToken)
            {
                return Task.FromResult(new DiscordInteractionResponseBuilder().Build());
            }
        }
    }
}
