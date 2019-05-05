using System.Linq;
using System.Threading.Tasks;
using Gommon;
using RestSharp;
using Volte.Commands;
using Volte.Data.Objects.EventArgs;
using Volte.Extensions;

namespace Volte.Services
{
    [Service("Antilink", "The main Service for checking links sent in chat.")]
    public sealed class AntilinkService
    {
        internal async Task CheckMessageAsync(MessageReceivedEventArgs args)
        {
            var m = args.Message.Content.Split(" ");
            if (m.Length < 1) m = new[] {args.Message.Content};

            if (!args.Config.ModerationOptions.Antilink || args.Context.User.IsAdmin()) return;

            foreach (var part in m)
            {
                if (!part.StartsWith("http://") && !part.StartsWith("https://")) continue;
                if (part.Contains("oauth2/authorize?client_id=")) continue;
                var resp = new RestClient(part).Execute(new RestRequest());

                var csp = resp.Headers.FirstOrDefault(x => x.Name.Equals("Content-Security-Policy"));

                if (csp != null && csp.Value.ToString().Contains("discord.gg"))
                {
                    await args.Message.DeleteAsync();
                    var warnMsg = await args.Context.CreateEmbed("Don't send server invites here.")
                        .SendToAsync(args.Context.Channel);
                    await Executor.ExecuteAfterDelayAsync(3000, async () => await warnMsg.DeleteAsync());
                }
            }
        }
    }
}