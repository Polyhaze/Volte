namespace Volte.Systems.Commands.Text.Modules;

public sealed partial class UtilityModule
{
    [Command("Tag")]
    [Description("Gets a tag's contents if it exists.")]
    public Task<ActionResult> TagAsync([Remainder, Description("The tag to show.")]
        Tag tag)
    {
        tag.Uses++;
        Db.Save(Context.GuildData);

        return Context.GuildData.Configuration.EmbedTagsAndShowAuthor
            ? Ok(tag.AsEmbed(Context))
            : Ok(tag.FormatContent(Context), shouldEmbed: false);
    }
}