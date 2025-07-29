namespace Volte.Entities;

public sealed class CommandBadRequestEventArgs : CommandEventArgs
{
    public BadRequestResult Result { get; }
    public ResultCompletionData ResultCompletionData { get; }

    public CommandBadRequestEventArgs(BadRequestResult res, ResultCompletionData data, CommandEventArgs args)
    {
        Result = res;
        ResultCompletionData = data;
        Context = args.Context;
        Arguments = args.Arguments;
        Command = args.Command;
        Stopwatch = args.Stopwatch;
    }
    
    public string FormatResult()
        => Fmt(Executed, $"{Result.IsSuccessful} | Reason: {Result.Reason}");

    public string FormatCommandResultMessage()
        => Fmt(ResultMessage, $"{ResultCompletionData.Message?.Id}");
}