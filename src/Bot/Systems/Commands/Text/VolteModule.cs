using Volte.Systems.Database;
using Volte.Systems.Interactive;
using Volte.Systems.Moderation;

namespace Volte.Systems.Commands.Text;

public abstract class VolteModule : ModuleBase<VolteContext>
{
    public DatabaseService Db { get; set; }
    public CommandService CommandService { get; set; }
        
    protected static ActionResult Ok(
        string text, 
        MessageCallback afterCompletion = null,
        bool shouldEmbed = true) 
        => new OkResult(text, shouldEmbed, null, afterCompletion);
    
    protected static ActionResult Ok(
        string text, 
        ModActionEventArgs modAction,
        bool shouldEmbed = true) 
        => new OkResult(text, shouldEmbed, null, _ =>
        {
            ModerationService.CallEvent(modAction);
            return Task.CompletedTask;
        });

    protected static ActionResult Ok(
        Action<StringBuilder> textBuilder,
        ModActionEventArgs modAction,
        bool shouldEmbed = true)
        => Ok(String(textBuilder), modAction, shouldEmbed);

    protected static ActionResult Ok(
        AsyncFunction logic, 
        bool awaitLogic = true) 
        => new OkResult(logic, awaitLogic);

    protected static ActionResult Ok(Action<StringBuilder> textBuilder, MessageCallback messageCallback = null,
        bool shouldEmbed = true)
        => Ok(String(textBuilder), messageCallback, shouldEmbed);

    protected static ActionResult Ok(StringBuilder text, MessageCallback messageCallback = null, bool shouldEmbed = true)
        => Ok(text.ToString(), messageCallback, shouldEmbed);
    protected static ActionResult Ok(PaginatedMessage.Builder pager) => new OkResult(pager);
    protected static ActionResult Ok(IEnumerable<EmbedBuilder> embeds) => new OkResult(embeds);

    protected static ActionResult Ok(PollInfo pollInfo) => new OkResult(pollInfo);

    protected static ActionResult Ok(
        EmbedBuilder embed, 
        MessageCallback afterCompletion = null) 
        => new OkResult(null, true, embed, afterCompletion);

    protected static ActionResult Ok(string text) 
        => new OkResult(text);

    protected static ActionResult Ok(EmbedBuilder embed) 
        => new OkResult(null, true, embed);

    protected static ActionResult BadRequest(string reason) 
        => new BadRequestResult(reason);

    protected static ActionResult None(
        AsyncFunction afterCompletion = null, 
        bool awaitCallback = true) 
        => new NoResult(afterCompletion, awaitCallback);
}