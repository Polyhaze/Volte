using System;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using BrackeysBot.Core.Models;

namespace BrackeysBot
{
    public class GuildUserProxyTypeReader : UserTypeReader<IGuildUser>
    {
        public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            // Try to get the result from the base user reader.
            // If that fails, try to solely parse the ID.

            TypeReaderResult result = await base.ReadAsync(context, input, services);
            if (result.IsSuccess)
            {
                IGuildUser user = result.BestMatch as IGuildUser;
                GuildUserProxy proxy = new GuildUserProxy
                {
                    GuildUser = user,
                    ID = user.Id
                };
                return TypeReaderResult.FromSuccess(proxy);
            }
            else
            {
                if (MentionUtils.TryParseUser(input, out ulong userId) || ulong.TryParse(input, out userId))
                    return TypeReaderResult.FromSuccess(new GuildUserProxy { ID = userId });
            }

            return TypeReaderResult.FromError(CommandError.ObjectNotFound, "User not found.");
        }
    }
}
