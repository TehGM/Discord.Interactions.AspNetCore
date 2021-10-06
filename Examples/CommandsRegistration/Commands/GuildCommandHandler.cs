using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TehGM.Discord.Interactions.CommandsHandling;

namespace TehGM.Discord.Interactions.Examples.CommandsRegistration.Commands
{
    // this command will be only available to specific guilds
    // this will also work if the command used builder method instead of [InteractionCommand] attribute
    [InteractionCommand("super-server", "This command is only available in super servers!")]
    [GuildInteractionCommand(886696135069155348)]
    public class GuildCommandHandler : IDiscordInteractionCommandHandler
    {
        public async Task<DiscordInteractionResponse> InvokeAsync(DiscordInteraction message, HttpRequest request, CancellationToken cancellationToken)
        {
            // \uD83D\uDE0E is the code for :sunglasses: emoji
            // find more on https://www.fileformat.info/info/emoji/list.htm
            return new DiscordInteractionResponseBuilder()
                .WithText("Welcome to the super server, where only super people reside. \uD83D\uDE0E")
                .Build();
        }
    }
}
