﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TehGM.Discord.Serialization;

namespace TehGM.Discord
{
    /// <summary>Discord allowed mentions object.</summary>
    /// <remarks>Do NOT serialize manually. Use <see cref="DiscordAllowedMentionsConverter"/>, which will handle any invalid state of the object.</remarks>
    public class DiscordAllowedMentions
    {
        public static DiscordAllowedMentions All { get; } = new DiscordAllowedMentions() { AllRoles = true, AllUsers = true, Everyone = true, Reply = true };
        public static DiscordAllowedMentions RepliedUser { get; } = new DiscordAllowedMentions() { Reply = true };
        public static DiscordAllowedMentions Users(IEnumerable<ulong> userIDs) => BuildNew(result => result.AddUserIDs(userIDs));
        public static DiscordAllowedMentions Users(params ulong[] userIDs) => Users(userIDs as IEnumerable<ulong>);
        public static DiscordAllowedMentions Roles(IEnumerable<ulong> roleIDs) => BuildNew(result => result.AddRoleIDs(roleIDs));
        public static DiscordAllowedMentions Roles(params ulong[] roleIDs) => Roles(roleIDs as IEnumerable<ulong>);

        [JsonProperty(DiscordAllowedMentionsConverter.ParsePropertyName, NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        private readonly HashSet<string> _parse;
        [JsonProperty(DiscordAllowedMentionsConverter.UsersPropertyName, NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IReadOnlyCollection<ulong> UserIDs { get; private set; }
        [JsonProperty(DiscordAllowedMentionsConverter.RolesPropertyName, NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IReadOnlyCollection<ulong> RoleIDs { get; private set; }
        [JsonIgnore]
        public bool AllUsers
        {
            get => this._parse.Contains(ParseValues.Users);
            set
            {
                if (value)
                    this._parse.Add(ParseValues.Users);
                else
                    this._parse.Remove(ParseValues.Users);
            }
        }
        [JsonIgnore]
        public bool AllRoles
        {
            get => this._parse.Contains(ParseValues.Roles);
            set
            {
                if (value)
                    this._parse.Add(ParseValues.Roles);
                else
                    this._parse.Remove(ParseValues.Roles);
            }
        }
        [JsonIgnore]
        public bool Everyone
        {
            get => this._parse.Contains(ParseValues.Everyone);
            set
            {
                if (value)
                    this._parse.Add(ParseValues.Everyone);
                else
                    this._parse.Remove(ParseValues.Everyone);
            }
        }

        [JsonProperty(DiscordAllowedMentionsConverter.ReplyPropertyName)]
        public bool Reply { get; set; }

        public DiscordAllowedMentions()
        {
            this._parse = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddUserIDs(params ulong[] ids)
            => AddUserIDs(ids as IEnumerable<ulong>);
        public void AddUserIDs(IEnumerable<ulong> ids)
        {
            if (ids?.Any() != true)
                return;
            if (this.UserIDs == null)
                this.UserIDs = new List<ulong>();
            (this.UserIDs as List<ulong>).AddRange(ids);
        }

        public void AddRoleIDs(params ulong[] ids)
            => AddRoleIDs(ids as IEnumerable<ulong>);
        public void AddRoleIDs(IEnumerable<ulong> ids)
        {
            if (ids?.Any() != true)
                return;
            if (this.RoleIDs == null)
                this.RoleIDs = new List<ulong>();
            (this.RoleIDs as List<ulong>).AddRange(ids);
        }

        // COMBINING HELPERS
        private static DiscordAllowedMentions BuildNew(Action<DiscordAllowedMentions> operation)
        {
            DiscordAllowedMentions result = new DiscordAllowedMentions();
            operation.Invoke(result);
            return result;
        }

        public static DiscordAllowedMentions operator +(DiscordAllowedMentions obj1, DiscordAllowedMentions obj2)
            => Combine(obj1, obj2);

        public static DiscordAllowedMentions Combine(params DiscordAllowedMentions[] allowedMentions)
            => BuildNew(result =>
            {
                result.Everyone = allowedMentions.Any(input => input.Everyone);
                result.AllRoles = allowedMentions.Any(input => input.AllRoles);
                result.AllUsers = allowedMentions.Any(input => input.AllUsers);
                result.Reply = allowedMentions.Any(input => input.Reply);
                result.AddUserIDs(allowedMentions.SelectMany(input => input.UserIDs));
                result.AddRoleIDs(allowedMentions.SelectMany(input => input.RoleIDs));
            });


        internal static class ParseValues
        {
            public const string Everyone = "everyone";
            public const string Users = "users";
            public const string Roles = "roles";
        }
    }
}
