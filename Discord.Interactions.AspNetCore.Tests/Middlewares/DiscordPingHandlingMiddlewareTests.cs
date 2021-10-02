using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

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
            HttpClient client = base.Host.GetTestClient();

            var response = await client.PostAsync("/api/discord/interactions", 
                new StringContent("{ type: 1 }", Encoding.UTF8, "application/json"));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task PingHandling_IgnoresNonPing()
        {
            HttpClient client = base.Host.GetTestClient();

            var response = await client.PostAsync("/api/discord/interactions", 
            new StringContent("{ type: 2 }", Encoding.UTF8, "application/json"));

            Assert.AreNotEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
