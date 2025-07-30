using Discord.Interactions;
using Volte;

namespace Gommon;

public static class InteractionContextExtensions
{
    public static Embed CreateEmbed<TInteraction>(
        this SocketInteractionContext<TInteraction> context,
        StringBuilder content
    ) where TInteraction : SocketInteraction
        => context.CreateEmbed(content.ToString());

    public static Embed CreateEmbed<TInteraction>(
        this SocketInteractionContext<TInteraction> context,
        Action<EmbedBuilder> action
    ) where TInteraction : SocketInteraction
        => context.CreateEmbedBuilder().Apply(action).Build();

    public static Embed CreateEmbed<TInteraction>(
        this SocketInteractionContext<TInteraction> context,
        string content
    ) where TInteraction : SocketInteraction
        => context.CreateEmbedBuilder(content).Build();

    public static EmbedBuilder CreateEmbedBuilder<TInteraction>(
        this SocketInteractionContext<TInteraction> context,
        string content = null
    ) where TInteraction : SocketInteraction
        => new EmbedBuilder()
            .WithColor(context.User.GetHighestRole()?.Color ?? Config.SuccessColor)
            .WithAuthor(context.User.ToString(), context.User.GetDisplayAvatarUrl())
            .WithDescription(content ?? string.Empty);
}