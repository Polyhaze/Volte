using System.Linq;
using Gommon;
using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public partial class UtilityModule
    {
        [Command("Nato")]
        [Description("Converts a given String to the NATO-phonetic alphabet.")]
        [Remarks("nato {String}")]
        public Task<ActionResult> NatoAsync([Remainder] string input)
        {
            var arr = input.ToLower().ToCharArray().Where(x => !x.Equals(' '));
            var l = arr.Select(GetNato);
            return Ok($"Result: ```{l.Join(" ")}```\n\n{EmojiService.Repeat} Original: ```{input}```");
        }
    }
}
