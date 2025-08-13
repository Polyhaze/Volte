namespace Volte.Systems.Commands.Text.Modules;

public partial class ModerationModule
{
    [Command("PirateBan", "PBan")]
    [Description("Bans the user with a very long-winded message as to why piracy is not supported. Content is retrieved from the 'emulationisnotpiracy' tag.")]
    [RequireSpecificGuild([1294443224030511104, 1325735818714943498])]
    public async Task<ActionResult> PirateBanAsync(
        [CheckHierarchy, EnsureNotSelf, Description("The target user.")] RestUser user)
    {
        bool sentReasonDm = false;
        
        if (Context.Guild.GetUser(user.Id) is { } member)
        {
            var e = Context.CreateEmbedBuilder($"You've been banned from **{Context.Guild.Name}** for **piracy**.");
        
            if (!await member.TrySendMessageAsync(embed: e.Apply(Context.GuildData).Build()))
                Warn(LogSource.Module, $"encountered a 403 when trying to message {member}!");

            var tag = Context.GuildData.GetTagByNameOrAlias("emulationisnotpiracy");
            if (tag.HasValue)
            {
                if (tag.Value.Response.Length <= 2000)
                {
                    if (!(sentReasonDm = await member.TrySendMessageAsync(text: tag.Value.Response)))
                        Warn(LogSource.Module, $"encountered a 403 when trying to message {member}!");
                }
                else
                {
                    if (!(sentReasonDm = await member.TrySendMessageAsync(embed: tag.Value.AsContentEmbed(Context).Build())))
                        Warn(LogSource.Module, $"encountered a 403 when trying to message {member}!");
                }

                if (sentReasonDm)
                {
                    tag.Value.Uses++;
                    Db.Save(Context.GuildData);
                }
            }
        }

        try
        {
            await Context.Guild.AddBanAsync(user, 7, GetReason("Banned", Context.User, "Piracy"));
            return Ok(sb =>
                {
                    sb.Append($"Successfully banned **{user}** from this guild.");

                    if (sentReasonDm)
                        sb.AppendLine().Append("They were also DMed a verbose explainer.");
                }, Context.ModAction(ModActionType.Ban)
                .WithTarget(user)
                .WithReason("Piracy")
            );
        }
        catch
        {
            return BadRequest(
                "An error occurred banning that member. Do I have permission; or are they higher than me in the role list?");
        }
    }
}