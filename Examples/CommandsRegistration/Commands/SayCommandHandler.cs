using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TehGM.Discord.Interactions.CommandsHandling;

namespace TehGM.Discord.Interactions.Examples.CommandsRegistration
{
    // this example uses a static build method to provide info for command registration
    public class SayCommandHandler : IDiscordInteractionCommandHandler
    {
        // mark method with the attribute
        // only one method can be marked. This method must be static, and return DiscordApplicationCommand or Task<DiscordApplicationCommand>
        // as an example here, we inject ILogger<SayCommandHandler> - but it's optional. The method can also request other registered services, as well as CancellationToken.
        [InteractionCommandBuilder]
        static DiscordApplicationCommand Build(ILogger<SayCommandHandler> log)
        {
            // you can use any of the injected services here
            log.LogDebug("Building command {Command}", "/say");

            // use builder utility to simplify command building
            return DiscordApplicationCommandBuilder.CreateSlashCommand("say", "Make me say something!")
                .AddOption(option =>
                {
                    option.Name = "text";
                    option.Description = "The text you want me to say.";
                    option.Type = DiscordApplicationCommandOptionType.String;
                    option.IsRequired = true;
                })
                .AddOption(option =>
                {
                    option.Name = "embed";
                    option.Description = "Should I embed the thing you want me to say?";
                    option.Type = DiscordApplicationCommandOptionType.Boolean;
                    option.IsRequired = false;
                })
                .Build();
        }

        // process data and send the response
        public async Task<DiscordInteractionResponse> InvokeAsync(DiscordInteraction message, HttpRequest request, CancellationToken cancellationToken)
        {
            DiscordInteractionResponseBuilder response = new DiscordInteractionResponseBuilder();
            // use Data.TryGetOption methods to retrieve the values provided by the user
            if (message.Data.TryGetStringOption("text", out string text))
            {
                // check that user set value of "embed" option, and if it's also true
                if (message.Data.TryGetBooleanOption("embed", out bool embed) && embed)
                {
                    response.AddEmbed(embed =>
                    {
                        embed.WithDescription(text);
                    });
                }
                else
                {
                    response.WithText(text);
                }
            }
            else
            {
                response.WithText("You can't ask me to say nothing!");
                // when response is ephemeral, only the user that sent the message can see it
                response.WithEphemeral(true);
            }
            return response.Build();
        }
    }
}
