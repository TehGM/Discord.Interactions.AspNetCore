using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TehGM.Discord.Interactions.CommandsHandling;

namespace TehGM.Discord.Interactions.Examples.CommandsRegistration
{
    // this command uses attribute to provide info for registration code
    [InteractionCommand("ping", "Pings me!")]
    public class PingCommandHandler : IDiscordInteractionCommandHandler
    {
        public async Task<DiscordInteractionResponse> InvokeAsync(DiscordInteraction message, HttpRequest request, CancellationToken cancellationToken)
        {
            // use GetUser to get user object independent if the command was invoked from DM or Guild
            DiscordUser user = message.GetUser();

            // use the builder to simplify the creation of response
            return new DiscordInteractionResponseBuilder()
                .WithText($"Pong! {DiscordFormatter.MentionUser(user.ID)}")
                .Build();
        }
    }
}
