using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using TehGM.Discord.Interactions.CommandsHandling;

namespace TehGM.Discord.Interactions
{
    /// <summary>Application options for Discord Interactions.</summary>
    public class DiscordInteractionsOptions
    {
        /// <summary>Discord application Public Key.</summary>
        /// <remarks>Used for signature verification.</remarks>
        /// <see href="https://discord.com/developers/applications"/>
        public string PublicKey { get; set; }

        /// <summary>The user agent to use when making requests to Discord API servers.</summary>
        public string UserAgent { get; set; } = $"Discord.Interactions.AspNetCore (v{GetVersion()})";
        /// <summary>The bot token to authenticate with when making requests to Discord API servers.</summary>
        public string BotToken { get; set; }
        /// <summary>Base API URL for API requests.</summary>
        public string BaseApiURL { get; set; } = "https://discord.com/api/v9/";

        private static string GetVersion()
        {
            FileVersionInfo version = FileVersionInfo.GetVersionInfo(typeof(DiscordInteractionsOptions).Assembly.Location);

            string result = $"{version.ProductMajorPart}.{version.ProductMinorPart}.{version.ProductBuildPart}";
            if (version.FilePrivatePart != 0)
                result += $" r{version.FilePrivatePart}";

            return result;
        }
    }
}
