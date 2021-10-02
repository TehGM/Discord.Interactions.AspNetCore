using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace TehGM.Discord.Interactions.AspNetCore.Tests
{
    [TestFixture]
    [Category("Middlewares")]
    public class DiscordSignatureVerificationMiddlewareTests : MiddlewareTestBase
    {
        // values pre-generated for this test
        // see Utilities/Signing.cs for example how to generate these
        private const string _publicKey = "2321d324a396ca2d85ae82c3f6ad703aedc5f252d1f7dcdfde867e2b0942c92f";
        private const string _requestSignature = "ad384333dc54a6412b2fa43725f0f0018897c17263bf2c5d5ae31e4a0b5e1eaa9a02482b24df3f866e9070bfd4a66af64a2ed1a18f823fb30be58ad12571700f";

        private static readonly HttpContent _requestBody = new StringContent("{ foo: \"bar\" }", Encoding.UTF8, "application/json");

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DiscordInteractionsOptions>(options =>
            {
                options.PublicKey = _publicKey;
            });
        }

        protected override void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<DiscordSignatureVerificationMiddleware>();
        }

        [Test]
        public async Task SignatureVerification_MissingSignature_Fails()
        {
            HttpClient client = base.Host.GetTestClient();
            client.DefaultRequestHeaders.Add("X-Signature-Timestamp", "1633173350");

            var response = await client.PostAsync("/api/discord/interactions", _requestBody);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public async Task SignatureVerification_MissingTimestamp_Fails()
        {
            HttpClient client = base.Host.GetTestClient();
            client.DefaultRequestHeaders.Add("X-Signature-Ed25519", _requestSignature);

            var response = await client.PostAsync("/api/discord/interactions", _requestBody);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public async Task SignatureVerification_InvalidSignature_Fails()
        {
            HttpClient client = base.Host.GetTestClient();
            client.DefaultRequestHeaders.Add("X-Signature-Ed25519", "12608b72620b377483404a7b66bd496ef88b2b0e2258ade24b098e3e028d27653ece08b59087c1d86731424f53964562b0b7802dba9c688efe4e7740ca5cf20a");
            client.DefaultRequestHeaders.Add("X-Signature-Timestamp", "1633173350");

            var response = await client.PostAsync("/api/discord/interactions", _requestBody);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public async Task SignatureVerification_ValidSignature_Passes()
        {
            HttpClient client = base.Host.GetTestClient();
            client.DefaultRequestHeaders.Add("X-Signature-Ed25519", _requestSignature);
            client.DefaultRequestHeaders.Add("X-Signature-Timestamp", "1633173350");

            var response = await client.PostAsync("/api/discord/interactions", _requestBody);

            Assert.AreNotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}