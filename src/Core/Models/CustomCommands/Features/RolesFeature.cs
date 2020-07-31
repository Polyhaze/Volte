using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.Commands;

namespace BrackeysBot.Core.Models
{
    [Name("Roles")]
    [Summary("Adds or removes a role from a user.")]
    public class RolesFeature : CustomCommandFeature
    {
        public string[] Operations { get; set; }

        public override void FillArguments(string arguments)
        {
            Operations = arguments.Split(',')
                .Select(operation => operation.Trim())
                .ToArray();

            if (Operations.Any(o => !o.StartsWith("+") && !o.StartsWith("-")))
                throw new ArgumentException("Invalid role operations.");
        }
        public override async Task Execute(ICommandContext context)
        {
            if (context.Guild == null)
                throw new Exception("Role commands can only be called in a guild!");

            IGuildUser user = context.User as IGuildUser;

            List<string> successAdd = new List<string>();
            foreach (IRole role in GetRolesToAdd().Select(r => GetRole(r, context)))
            {
                if (role == null)
                    continue;

                if (!user.RoleIds.Contains(role.Id))
                {
                    await user.AddRoleAsync(role);
                    successAdd.Add(role.Name);
                }
            }

            List<string> successRemove = new List<string>();
            foreach (IRole role in GetRolesToRemove().Select(r => GetRole(r, context)))
            {
                if (role == null)
                    continue;

                if (user.RoleIds.Contains(role.Id))
                {
                    await user.RemoveRoleAsync(role);
                    successRemove.Add(role.Name);
                }
            }

            StringBuilder reply = new StringBuilder();
            if (successAdd.Count > 0)
            {
                reply.Append("You now have the roles: ")
                    .AppendJoin(", ", successAdd)
                    .AppendLine();
            }
            if (successRemove.Count > 0)
            {
                reply.Append("You no longer have the roles: ")
                    .AppendJoin(", ", successRemove);
            }

            await new EmbedBuilder()
                .WithColor(reply.Length > 0 ? Color.Green : Color.Red)
                .WithDescription(reply.ToString().WithAlternative("No roles have been added or removed!"))
                .Build()
                .SendToChannel(context.Channel);
        }

        private IEnumerable<string> GetRolesToAdd()
            => Operations.Where(o => o.StartsWith("+")).Select(o => o.Substring(1));
        private IEnumerable<string> GetRolesToRemove()
            => Operations.Where(o => o.StartsWith("-")).Select(o => o.Substring(1));

        private IRole GetRole(string name, ICommandContext context)
            => context.Guild.Roles.FirstOrDefault(r => r.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            var add = GetRolesToAdd();
            if (add.Count() > 0)
            {
                builder.Append("Adds roles: ")
                    .AppendJoin(", ", add)
                    .AppendLine();
            }

            var remove = GetRolesToRemove();
            if (remove.Count() > 0)
            {
                builder.Append("Removes roles: ")
                    .AppendJoin(", ", remove);
            }

            return builder.ToString();
        }
    }
}
