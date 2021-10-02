namespace TehGM.Discord.Interactions
{
    /// <summary>Application options for Discord Interactions.</summary>
    public class DiscordInteractionsOptions
    {
        /// <summary>Discord application Public Key.</summary>
        /// <remarks>Used for signature verification.</remarks>
        /// <see href="https://discord.com/developers/applications"/>
        public string PublicKey { get; set; }
    }
}
