namespace Volte.Systems.Commands.Text.Modules;

public partial class ModerationModule
{
    public readonly Dictionary<Snowflake, Snowflake> GuildsToVerifiedSwitchOwnerRoleIds = new()
    {
        { 1294443224030511104, 1334992661198930001 }, // R
        { 1325735818714943498, 1354326584550625460 } // C
    };
    
    public readonly Dictionary<Snowflake, Snowflake> GuildsToPirateRoleIds = new()
    {
        { 1294443224030511104, 1298451667863470120 }, // R
        { 1325735818714943498, 1405060402362060911 } // C
    };
    
    [Command("Pirate", "NoSupport")]
    [Description("Warns the target user for piracy, gives them the server's pirate role, and, if they have it, takes away Verified Switch Owner.")]
    [RequireSpecificGuild([1294443224030511104, 1325735818714943498])]
    public async Task<ActionResult> PirateAsync([CheckHierarchy, EnsureNotSelf, Description("The target user.")] SocketGuildUser member)
    {
        await member.WarnAsync(Context, Context.Guild.Id is 1294443224030511104 ? "Rule #4" : "Piracy");

        var verifiedRole = GuildsToVerifiedSwitchOwnerRoleIds[Context.Guild.Id];
        
        if (member.HasRole(verifiedRole))
            await member.RemoveRoleAsync(verifiedRole);

        var pirateRole = GuildsToPirateRoleIds[Context.Guild.Id];
        
        if (!member.HasRole(pirateRole))
            await member.AddRoleAsync(pirateRole);

        return Ok($"Successfully warned **{member}** for piracy, and gave them <@&{pirateRole.Raw}>.",
            _ => ModerationService.OnModActionCompleteAsync(ModActionEventArgs.InContext(Context)
                .WithActionType(ModActionType.Warn)
                .WithTarget(member)
                .WithReason(Context.Guild.Id is 1294443224030511104 ? "#rules #4" : "Piracy")));
    }
}