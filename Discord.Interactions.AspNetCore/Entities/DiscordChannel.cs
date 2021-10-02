using System;
using Newtonsoft.Json;

namespace TehGM.Discord
{
    /// <summary>This is only partial - interactions only include a few properties anyway.</summary>
    public class DiscordChannel
    {
        [JsonProperty("id")]
        public ulong ID { get; private set; }
        [JsonProperty("type")]
        public DiscordChannelType Type { get; private set; }
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; private set; }
        [JsonProperty("permissions", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? Permissions { get; private set; }

        // thread only
        [JsonProperty("parent_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? ParentID { get; private set; }
        [JsonProperty("thread_metadata", NullValueHandling = NullValueHandling.Ignore)]
        public DiscordThreadMetadata ThreadMetadata { get; private set; }

        [JsonIgnore]
        public bool IsThread => this.ParentID != null && this.ThreadMetadata != null;

        [JsonConstructor]
        private DiscordChannel() { }
    }

    public class DiscordThreadMetadata
    {
        [JsonProperty("archived")]
        public bool IsArchived { get; private set; }
        [JsonProperty("auto_archive_duration")]
        public int AutoArchiveMinutes { get; private set; }
        [JsonProperty("archive_timestamp")]
        public DateTimeOffset ArchiveTimestamp { get; private set; }
        [JsonProperty("locked")]
        public bool IsLocked { get; private set; }
        [JsonProperty("invitable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsInvitable { get; private set; }

        [JsonConstructor]
        private DiscordThreadMetadata() { }
    }

    public enum DiscordChannelType
    {
        GuildText = 0,
        DM = 1,
        GuildVoice = 2,
        GroupDM = 3,
        GuildCategory = 4,
        GuildNews = 5,
        GuildStore = 6,
        GuildNewsThread = 10,
        GuildPublicThread = 11,
        GuildPrivateThread = 12,
        GuildStageVoice = 13
    }
}
