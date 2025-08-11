using Volte.Systems.Database.EntitiesV2;

namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class UtilityModule
{
    [Command("Tag")]
    [Description("Gets a tag's contents if it exists.")]
    public Task<ActionResult> TagAsync([Remainder, Description("The tag to show.")] TagV2 tag)
    {
        tag.Uses++;
        Db.Save(Context.GuildData);

        return Context.GuildData.Settings.EmbedTags
            ? Ok(tag.AsEmbed(Context))
            : tag.FormatContent(Context)
                .Into(cont =>
                    Ok(cont, shouldEmbed: cont.Length > 2000)
                );
    }
}