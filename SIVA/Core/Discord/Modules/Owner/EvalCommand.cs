using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.VisualBasic.CompilerServices;
using SIVA.Helpers;
using Utils = SIVA.Helpers.Utils;

namespace SIVA.Core.Discord.Modules.Owner {
    public class EvalCommand : SIVACommand {
        [Command("Eval")]
        public async Task Eval(string code) {
            if (!UserUtils.IsBotOwner(Context.User)) {
                await Context.Message.AddReactionAsync(new Emoji(new RawEmoji().X));
            }

            var scriptOptions =
                ScriptOptions.Default.WithImports("System", "System.Threading.Tasks", "System.Collections.Generic", "System.IO");

            var scriptCompleted = CSharpScript.EvaluateAsync(code, scriptOptions).IsCompletedSuccessfully;
            if (scriptCompleted) {
                await Context.Channel.SendMessageAsync("", false,
                    Utils.CreateEmbed(Context, $"```cs\n{code}``` Executed successfully."));
            } else {
                await Context.Channel.SendMessageAsync("", false,
                    Utils.CreateEmbed(Context, $"{code} Failed to execute. Check your bot console."));
            }
            
        }
    }
}