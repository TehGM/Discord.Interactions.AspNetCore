using System.Security.Claims;

namespace TehGM.Discord.Interactions.AspNetCore.Authentication
{
    public static class DiscordInteractionsAuthenticationClaims
    {
        public const string UserID = ClaimTypes.NameIdentifier;
        public const string Username = ClaimTypes.Name;
        public const string UserDiscriminator = "urn:discord:user:discriminator";
        public const string UserPublicFlags = "urn:discord:user:public_flags";

        public const string InteractionID = "urn:discord:interaction:id";
        public const string ApplicationID = "urn:discord:interaction:application_id";
    }
}
