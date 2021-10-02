using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TehGM.Discord.Interactions.CommandsHandling
{
    /// <summary>Represents an instance of a class that can be invoked by a Discord Interaction.</summary>
    public interface IDiscordInteractionCommand
    {
        /// <summary>Invokes the command.</summary>
        /// <param name="message">Received interaction.</param>
        /// <param name="request">HTTP Request of the interaction.</param>
        /// <param name="services">Service provider that can be used to retrieve DI services.</param>
        /// <param name="cancellationToken">Cancellation token used to cancel operations.</param>
        /// <returns>Response to the interaction.</returns>
        Task<DiscordInteractionResponse> InvokeAsync(DiscordInteraction message, HttpRequest request, IServiceProvider services, CancellationToken cancellationToken);
    }
}
