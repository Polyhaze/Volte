namespace Volte.Systems.Commands.Text;

public class ResultCompletionData
{
    public IUserMessage Message { get; }

    public ResultCompletionData(IUserMessage message) 
        => Message = message;

    public static implicit operator ValueTask<ResultCompletionData>(ResultCompletionData data) 
        => new(data);
}