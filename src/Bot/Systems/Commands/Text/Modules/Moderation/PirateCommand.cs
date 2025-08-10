namespace Volte.Systems.Commands.Text.Modules;

public partial class ModerationModule
{
    public const ulong VerifiedSwitchOwnerRoleId = 1334992661198930001;
    public const ulong NoSupportRoleId = 1298451667863470120;
    
    [Command("Pirate", "NoSupport")]
    [Description("Warns the target user for Rule 4, gives them the No Support role, and, if they have it, takes away Verified Switch Owner.")]
    [RequireSpecificGuild(1294443224030511104)]
    public async Task<ActionResult> PirateAsync([Description("The target user.")] SocketGuildUser member)
    {
        await member.WarnAsync(Context, "Rule #4");
        if (member.HasRole(VerifiedSwitchOwnerRoleId))
            await member.RemoveRoleAsync(VerifiedSwitchOwnerRoleId);

        if (!member.HasRole(NoSupportRoleId))
            await member.AddRoleAsync(NoSupportRoleId);

        return Ok($"Successfully warned **{member}** for piracy, and gave them <@&1298451667863470120>.", Context.ModAction
            .WithActionType(ModActionType.Warn)
            .WithTarget(member)
            .WithReason("#rules #4")
        );
    }
}