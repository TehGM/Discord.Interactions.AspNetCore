using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TehGM.Discord.Interactions.Examples.CommandsRegistration
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // load options
            // see appsettings.json file
            services.Configure<DiscordInteractionsOptions>(Configuration.GetSection("Discord"));

            // add interactions services
            services.AddDiscordInteractions(options =>
            {
                // enable commands registration
                // these options can also be set in appsettings.json
                options.RegisterCommands = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // use interactions middlewares
            app.UseDiscordInteractions();
            app.UseRouting();
        }
    }
}
