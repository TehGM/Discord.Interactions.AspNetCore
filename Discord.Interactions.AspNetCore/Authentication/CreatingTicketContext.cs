using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace TehGM.Discord.Interactions.AspNetCore.Authentication
{
    public class CreatingTicketContext : ResultContext<DiscordInteractionsAuthenticationOptions>
    {
        public JToken UserJson { get; }
        public JToken GuildMemberJson { get; }
        public ClaimsIdentity Identity => base.Principal?.Identity as ClaimsIdentity;

        public CreatingTicketContext(HttpContext context, AuthenticationScheme scheme, DiscordInteractionsAuthenticationOptions options,
            ClaimsPrincipal principal, JToken userJson, JToken guildMemberJson)
            : base(context, scheme, options)
        {
            this.UserJson = userJson;
            this.GuildMemberJson = guildMemberJson;
            base.Principal = principal;
        }
    }
}
