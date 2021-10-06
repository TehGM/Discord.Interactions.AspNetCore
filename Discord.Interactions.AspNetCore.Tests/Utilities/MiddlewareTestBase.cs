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
        public virtual Task Setup()
            => this.BuildHostAsync(ConfigureServices, Configure);
        protected async Task BuildHostAsync(Action<IServiceCollection> configureServices, Action<IApplicationBuilder> configure)
        {
            if (this.Host != null)
                await this.TearDown();

            this.Host = await new HostBuilder()
                .ConfigureWebHost(builder =>
                {
                    builder.UseTestServer();
                    if (configureServices != null)
                        builder.ConfigureServices(configureServices);
                    if (configure != null)
                        builder.Configure(configure);
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
