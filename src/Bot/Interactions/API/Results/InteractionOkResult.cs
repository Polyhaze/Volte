namespace Volte.Interactions.Results;

public class InteractionOkResult<TInteraction> : InteractionResultBase where TInteraction : SocketInteraction
{
    public InteractionOkResult(ReplyBuilder<TInteraction> reply)
    {
        Reply = reply;
    }
    
    public AsyncFunction AfterCompletion { get; set; }

    public readonly ReplyBuilder<TInteraction> Reply;

    public override async Task ExecuteAsync()
    {
        await Reply.ExecuteAsync();

        if (AfterCompletion != null)
            await AfterCompletion();
    }
}