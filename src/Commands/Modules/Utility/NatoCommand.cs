using System;
using System.Collections.Generic;
using System.Linq;
using Gommon;
using System.Threading.Tasks;
using Qmmands;
using Volte.Commands.Results;

namespace Volte.Commands.Modules
{
    public sealed partial class UtilityModule
    {
        [Command("Nato")]
        [Description("Translates a string into the NATO Phonetic Alphabet. If no string is provided, then a full rundown of the NATO alphabet is shown.")]
        [Remarks("nato [String]")]
        public Task<ActionResult> NatoAsync([Remainder] string input = null)
        {
            if (input.IsNullOrEmpty())
                return None(async () =>
                {
                    await Context.Interactivity.SendPaginatedMessageAsync(Context.Channel, Context.Member,
                        _nato.Select(x => $"**{x.Key.ToString().ToUpper()}**: `{x.Value}`").GetPages(10));
                }, false);

            // ReSharper disable once PossibleNullReferenceException
            //this legit cant happen because of the if statement above
            var arr = input.ToLower().ToCharArray().Where(x => !x.Equals(' '));
            var l = new List<string>();

            foreach (var ch in arr)
            {
                try
                {
                    l.Add(GetNato(ch));
                }
                catch (InvalidOperationException)
                {
                    return BadRequest(
                        $"There is not a NATO word for the character `{ch}`. Only standard English letters and numbers are valid.");
                }
            }

            return Ok(Context.CreateEmbedBuilder().AddField("Result", $"`{l.Join(" ")}`")
                .AddField("Original", $"`{input}`"));
        }
    }
}