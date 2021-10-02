using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace TehGM.Discord.Interactions.AspNetCore.Tests
{
    [TestFixture]
    [Category("Middlewares")]
    class DiscordInteractionReaderMiddlewareTests : MiddlewareTestBase
    {
        protected override void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<DiscordInteractionReaderMiddleware>();
        }

        private void BuildRequest(HttpContext context, string body)
        {
            context.Request.Method = HttpMethods.Post;
            context.Request.Path = "/api/discord/interactions";
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(body));
            context.Request.Body = stream;
            context.Request.ContentLength = stream.Length;
        }

        [Test]
        public async Task InteractionReader_CreatesFeature_WithRaw()
        {
            var server = base.Host.GetTestServer();

            var context = await server.SendAsync(ctx => BuildRequest(ctx, "{ type: 1 }"));

            Assert.IsNotNull(context.Features.Get<IDiscordInteractionReaderFeature>().InteractionRaw);
        }

        [Test]
        public async Task InteractionReader_CreatesFeature_WithJson()
        {
            var server = base.Host.GetTestServer();

            var context = await server.SendAsync(ctx => BuildRequest(ctx, "{ type: 1 }"));

            Assert.IsNotNull(context.Features.Get<IDiscordInteractionReaderFeature>().InteractionJson);
            Assert.AreEqual(context.Features.Get<IDiscordInteractionReaderFeature>()
                .InteractionJson.Value<int>("type"), 1);
        }

        [Test]
        public async Task InteractionReader_CreatesFeature_WithTyped()
        {
            var server = base.Host.GetTestServer();

            var context = await server.SendAsync(ctx => BuildRequest(ctx, "{ type: 1 }"));

            Assert.IsNotNull(context.Features.Get<IDiscordInteractionReaderFeature>().Interaction);
            Assert.AreEqual(context.Features.Get<IDiscordInteractionReaderFeature>()
                .Interaction.Type, (DiscordInteractionType)1);
        }

        [Test]
        public async Task InteractionReader_InvalidData_FailsIfNoData()
        {
            var server = base.Host.GetTestServer();

            var context = await server.SendAsync(ctx => BuildRequest(ctx, ""));

            CustomAssert.CatchOrNull(() => context.Features.Get<IDiscordInteractionReaderFeature>().InteractionRaw);
        }

        [Test]
        public async Task InteractionReader_InvalidData_FailsIfInvalidJson_Json()
        {
            var server = base.Host.GetTestServer();

            var context = await server.SendAsync(ctx => BuildRequest(ctx, "absdef"));

            CustomAssert.CatchOrNull(() => context.Features.Get<IDiscordInteractionReaderFeature>().InteractionJson);
        }

        [Test]
        public async Task InteractionReader_InvalidData_FailsIfInvalidJson_Typed()
        {
            var server = base.Host.GetTestServer();

            var context = await server.SendAsync(ctx => BuildRequest(ctx, "absdef"));

            CustomAssert.CatchOrNull(() => context.Features.Get<IDiscordInteractionReaderFeature>().Interaction);
        }
    }
}
