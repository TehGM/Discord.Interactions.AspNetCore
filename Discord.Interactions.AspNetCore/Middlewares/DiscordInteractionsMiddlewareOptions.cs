using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Builder
{
    public class DiscordInteractionsMiddlewareOptions
    {
        public ICollection<string> Routes { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "/api/discord/interactions" };
        public bool HandlePings { get; set; } = true;
    }
}
