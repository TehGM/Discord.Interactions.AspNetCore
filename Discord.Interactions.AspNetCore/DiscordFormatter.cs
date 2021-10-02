using System;
using System.Collections.Generic;
using System.Text;
using TehGM.Discord.Serialization;

namespace TehGM.Discord
{
    public static class DiscordFormatter
    {
        // escaping
        private static HashSet<char> _escapeChars = new HashSet<char> { '*', '`', '<', '[', '~', '_' };
        public static string Escape(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            StringBuilder builder = new StringBuilder(text);
            for (int i = 0; i < builder.Length; i++)
            {
                char c = builder[i];
                if (!_escapeChars.Contains(c))
                    continue;

                builder.Insert(i, '\\');
                i++;
            }
            return builder.ToString();
        }

        // general
        public static string NamedLink(string name, string url)
            => $"[{name}]({url})";

        public static string Italic(string text)
            => $"*{text}*";
        public static string Bold(string text)
            => $"**{text}**";
        public static string BoldItalic(string text)
            => Bold(Italic(text));

        public static string Strikeout(string text)
            => $"~~{text}~~";

        public static string Underline(string text)
            => $"__{text}__";

        public static string Quote(string text)
            => $"> {text}";
        public static string MultilineQuote(string text)
            => $">>> {text}";

        public static string InlineCode(string code)
            => $"`{code}`";

        public static string CodeBlock(string code, string language)
            => $"```{(string.IsNullOrWhiteSpace(language) ? string.Empty : language)}\n{code}\n```";
        public static string CodeBlock(string code)
            => CodeBlock(code, null);

        // mentions
        public static string MentionUser(ulong id, bool withNickname = true)
        {
            if (withNickname)
                return $"<@!{id}>";
            else
                return $"<@{id}>";
        }
        public static string MentionChannel(ulong id)
            => $"<#{id}>";
        public static string MentionRole(ulong id)
            => $"<@&{id}>";

        // emojis
        public static string CustomEmoji(string name, ulong id)
            => $"<:{name}:{id}>";
        public static string CustomAnimatedEmoji(string name, ulong id)
            => $"<a:{name}:{id}>";

        // timestamp
        public static string Timestamp(DateTime timestamp, TimestampStyle style = TimestampStyle.ShortDateTime)
        {
            char styleChar;
            switch (style)
            {
                case TimestampStyle.ShortTime:
                    styleChar = 't';
                    break;
                case TimestampStyle.LongTime:
                    styleChar = 'T';
                    break;
                case TimestampStyle.ShortDate:
                    styleChar = 'd';
                    break;
                case TimestampStyle.LongDate:
                    styleChar = 'D';
                    break;
                case TimestampStyle.ShortDateTime:
                    styleChar = 'f';
                    break;
                case TimestampStyle.LongDateTime:
                    styleChar = 'F';
                    break;
                case TimestampStyle.RelativeTime:
                    styleChar = 'R';
                    break;
                default:
                    throw new ArgumentException("Unrecognized timestamp style", nameof(style));
            }

            return $"<t:{UnixTimestampConverter.ToUnixTimestamp(timestamp)}:{styleChar}>";
        }
        public static string Timestamp(DateTimeOffset timestamp, TimestampStyle style = TimestampStyle.ShortDateTime)
            => Timestamp(timestamp.UtcDateTime, style);

        public enum TimestampStyle
        {
            /// <summary>16:20</summary>
            ShortTime,
            /// <summary>16:20:30</summary>
            LongTime,
            /// <summary>20/04/2021</summary>
            ShortDate,
            /// <summary>20 April 2021</summary>
            LongDate,
            /// <summary>20 April 2021 16:20</summary>
            ShortDateTime,
            /// <summary>Tuesday, 20 April 2021 16:20</summary>
            LongDateTime,
            /// <summary>2 months ago</summary>
            RelativeTime
        }
    }
}
