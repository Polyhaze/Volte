namespace Volte.Interactions.Results;

public class InteractionOkResult<TInteraction> : InteractionResultBase where TInteraction : SocketInteraction
{
    public InteractionOkResult(ReplyBuilder<TInteraction> reply)
    {
        Reply = reply;
    }

    public readonly ReplyBuilder<TInteraction> Reply;

    public override Task ExecuteAsync() => Reply.ExecuteAsync();
}