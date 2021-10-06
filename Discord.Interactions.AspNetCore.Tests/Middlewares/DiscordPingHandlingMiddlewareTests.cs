using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using TehGM.Discord.Interactions.AspNetCore.Services;

namespace TehGM.Discord.Interactions.AspNetCore.Tests
{
    [TestFixture]
    [Category("Middlewares")]
    public class DiscordPingHandlingMiddlewareTests : MiddlewareTestBase
    {
        protected override void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<DiscordPingHandlingMiddleware>();
        }

        [Test]
        public async Task PingHandling_HandlesPing()
        {
            var server = base.Host.GetTestServer();
            var feature = new DiscordInteractionReaderFeature("{ type: 1 }");

            var context = await server.SendAsync(ctx =>
                ctx.Features.Set<IDiscordInteractionReaderFeature>(feature));

            Assert.AreEqual((int)HttpStatusCode.OK, context.Response.StatusCode);
            Assert.AreEqual("application/json", context.Response.Headers["Content-Type"]);
        }

        [Test]
        public async Task PingHandling_IgnoresNonPing()
        {
            var server = base.Host.GetTestServer();
            var feature = new DiscordInteractionReaderFeature("{ type: 2 }");

            var context = await server.SendAsync(ctx =>
                ctx.Features.Set<IDiscordInteractionReaderFeature>(feature));

            Assert.AreNotEqual((int)HttpStatusCode.OK, context.Response.StatusCode);
        }

        [Test]
        public async Task PingHandling_IgnoresIfDisabled()
        {
            // rebuild host with new options
            await base.BuildHostAsync(services => services.Configure<DiscordInteractionsOptions>(options => options.HandlePings = false), this.Configure);
            var server = base.Host.GetTestServer();
            var feature = new DiscordInteractionReaderFeature("{ type: 1 }");

            var context = await server.SendAsync(ctx =>
                ctx.Features.Set<IDiscordInteractionReaderFeature>(feature));

            Assert.AreNotEqual((int)HttpStatusCode.OK, context.Response.StatusCode);
        }
    }
}
