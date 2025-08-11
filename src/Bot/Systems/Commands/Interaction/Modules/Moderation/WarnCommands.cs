using Discord.Interactions;

namespace Volte.Systems.Commands.Interaction.Modules;

public partial class InteractionModerationModule
{
    [SlashCommand("warn", "Warns the target member for the given reason.")]
    public async Task<RuntimeResult> WarnAsync(
        [Summary("member", "The member to warn."), DoHierarchyCheck]
        SocketGuildUser member, 
        [Summary("reason", "The reason for the warn.")]
        string reason)
    {
        await Db.WarnAsync(Context.User, member, reason);
        
        return Ok($"Successfully warned **{member}** for **{reason}**.", 
            ModAction(ModActionType.Warn)
                .WithTarget(member)
                .WithReason(reason)
        );
    }
    
    [SlashCommand("clear_warns", "Clears the warnings for the given member.")]
    public async Task<RuntimeResult> ClearWarnsAsync(
        [Summary("member", "The member who you want to clear warns for."), DoHierarchyCheck]
        SocketGuildUser member)
    {
        var gd = GetData();
        var warnCount = gd.Moderation.Warns.RemoveWhere(x => x.Target == member.Id);
        Db.Save(gd);

        var e = Context
            .CreateEmbedBuilder(
                $"Your {"warn".ToQuantity(warnCount)} in {Format.Bold(Context.Guild.Name)} have been cleared. Hooray!")
            .Apply(gd);

        if (!await member.TrySendMessageAsync(embed: e.Build()))
            Warn(LogSource.Volte, $"encountered a 403 when trying to message {member}!");

        return Ok($"Cleared **{warnCount}** warnings for **{member}**.", 
            ModAction(ModActionType.ClearWarns).WithTarget(member)
        );
    }
}