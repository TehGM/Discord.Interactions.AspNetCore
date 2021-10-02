using TehGM.Discord.Interactions.AspNetCore.Authentication;

namespace Microsoft.AspNetCore.Authorization
{
    public class AuthorizeDiscordInteractionAttribute : AuthorizeAttribute
    {
        public AuthorizeDiscordInteractionAttribute()
        {
            base.AuthenticationSchemes = DiscordInteractionsAuthenticationDefaults.AuthenticationScheme;
        }
    }
}
