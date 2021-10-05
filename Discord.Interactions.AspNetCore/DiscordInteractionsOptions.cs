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
        /// <seealso href="https://discord.com/developers/applications"/>
        public string PublicKey { get; set; }
        /// <summary>Discord application Application ID. Required for commands registration.</summary>
        /// <remarks>Used for signature verification.</remarks>
        /// <seealso href="https://discord.com/developers/applications"/>
        public string ApplicationID { get; set; }

        /// <summary>The user agent to use when making requests to Discord API servers. Required for commands registration.</summary>
        public string UserAgent { get; set; } = $"Discord.Interactions.AspNetCore (v{GetVersion()})";
        /// <summary>The bot token to authenticate with when making requests to Discord API servers. Required for commands registration.</summary>
        /// <seealso href="https://discord.com/developers/applications"/>
        public string BotToken { get; set; }
        /// <summary>Base API URL for API requests. Required for commands registration.</summary>
        public string BaseApiURL { get; set; } = "https://discord.com/api/v9/";

        /// <summary>Whether the commands should be automatically registered.</summary>
        /// <remarks><para>Registration of commands overwrites all previously registered commands. For this reason, it's opt-in.</para>
        /// <para>However, if this option is disabled, the library won't support any <see cref="IDiscordInteractionCommandHandler"/> - manual registration or handling commands in a controller will be required.</para></remarks>
        public bool RegisterCommands { get; set; } = false;

        /// <summary>Set of assemblies interaction commands should be automatically loaded from.</summary>
        /// <remarks>Entry assembly is included by default.</remarks>
        public ICollection<Assembly> CommandAssemblies { get; set; } = new List<Assembly>() { Assembly.GetEntryAssembly() };

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
