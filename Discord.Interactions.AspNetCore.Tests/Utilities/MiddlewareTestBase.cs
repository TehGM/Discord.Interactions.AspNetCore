using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace TehGM.Discord.Interactions.AspNetCore.Tests
{
    public abstract class MiddlewareTestBase
    {
        protected IHost Host { get; private set; }

        [SetUp]
        public virtual async Task Setup()
        {
            this.Host = await new HostBuilder()
                .ConfigureWebHost(builder =>
                {
                    builder.UseTestServer();
                    builder.ConfigureServices(ConfigureServices);
                    builder.Configure(Configure);
                })
                .StartAsync();
        }

        protected abstract void Configure(IApplicationBuilder app);
        protected virtual void ConfigureServices(IServiceCollection services) { }

        [TearDown]
        public virtual async Task TearDown()
        {
            await this.Host?.StopAsync();
            this.Host?.Dispose();
        }
    }
}
