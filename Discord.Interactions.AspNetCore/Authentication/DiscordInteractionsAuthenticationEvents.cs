using System;
using System.Threading.Tasks;

namespace TehGM.Discord.Interactions.AspNetCore.Authentication
{
    public class DiscordInteractionsAuthenticationEvents
    {
        public Func<CreatingTicketContext, Task> OnCreatingTicket { get; set; } = _ => Task.CompletedTask;

        public virtual Task CreatingTicket(CreatingTicketContext context) => this.OnCreatingTicket?.Invoke(context);
    }
}
