using System;
using System.Linq;
using System.Text;
using Gommon;

namespace Volte.Core.Helpers
{
    [Flags]
    public enum IncludeChars
    {
        Up,
        Middle,
        Down
    }

    public enum ZalgoIntensity
    {
        Low,
        Medium,
        High
    }

    public static class ZalgoHelper
    {
        internal static readonly char[] UpChars =
        {
            '\u030d', /*     ̍     */ '\u030e', /*     ̎     */ '\u0304', /*     ̄     */ '\u0305', /*     ̅     */
            '\u033f', /*     ̿     */ '\u0311', /*     ̑     */ '\u0306', /*     ̆     */ '\u0310', /*     ̐     */
            '\u0352', /*     ͒     */ '\u0357', /*     ͗     */ '\u0351', /*     ͑     */ '\u0307', /*     ̇     */
            '\u0308', /*     ̈     */ '\u030a', /*     ̊     */ '\u0342', /*     ͂     */ '\u0343', /*     ̓     */
            '\u0344', /*     ̈́     */ '\u034a', /*     ͊     */ '\u034b', /*     ͋     */ '\u034c', /*     ͌     */
            '\u0303', /*     ̃     */ '\u0302', /*     ̂     */ '\u030c', /*     ̌     */ '\u0350', /*     ͐     */
            '\u0300', /*     ̀     */ '\u0301', /*     ́     */ '\u030b', /*     ̋     */ '\u030f', /*     ̏     */
            '\u0312', /*     ̒     */ '\u0313', /*     ̓     */ '\u0314', /*     ̔     */ '\u033d', /*     ̽     */
            '\u0309', /*     ̉     */ '\u0363', /*     ͣ     */ '\u0364', /*     ͤ     */ '\u0365', /*     ͥ     */
            '\u0366', /*     ͦ     */ '\u0367', /*     ͧ     */ '\u0368', /*     ͨ     */ '\u0369', /*     ͩ     */
            '\u036a', /*     ͪ     */ '\u036b', /*     ͫ     */ '\u036c', /*     ͬ     */ '\u036d', /*     ͭ     */
            '\u036e', /*     ͮ     */ '\u036f', /*     ͯ     */ '\u033e', /*     ̾     */ '\u035b', /*     ͛     */
            '\u0346', /*     ͆     */ '\u031a' /*     ̚     */
        };

        internal static readonly char[] MidChars =
        {
            '\u0315', /*     ̕     */ '\u031b', /*     ̛     */ '\u0340', /*     ̀     */ '\u0341', /*     ́     */
            '\u0358', /*     ͘     */ '\u0321', /*     ̡     */ '\u0322', /*     ̢     */ '\u0327', /*     ̧     */
            '\u0328', /*     ̨     */ '\u0334', /*     ̴     */ '\u0335', /*     ̵     */ '\u0336', /*     ̶     */
            '\u034f', /*     ͏     */ '\u035c', /*     ͜     */ '\u035d', /*     ͝     */ '\u035e', /*     ͞     */
            '\u035f', /*     ͟     */ '\u0360', /*     ͠     */ '\u0362', /*     ͢     */ '\u0338', /*     ̸     */
            '\u0337', /*     ̷     */ '\u0361', /*     ͡     */ '\u0489' /*     ҉_     */
        };

        internal static readonly char[] DownChars =
        {
            '\u0316', /*     ̖     */ '\u0317', /*     ̗     */ '\u0318', /*     ̘     */ '\u0319', /*     ̙     */
            '\u031c', /*     ̜     */ '\u031d', /*     ̝     */ '\u031e', /*     ̞     */ '\u031f', /*     ̟     */
            '\u0320', /*     ̠     */ '\u0324', /*     ̤     */ '\u0325', /*     ̥     */ '\u0326', /*     ̦     */
            '\u0329', /*     ̩     */ '\u032a', /*     ̪     */ '\u032b', /*     ̫     */ '\u032c', /*     ̬     */
            '\u032d', /*     ̭     */ '\u032e', /*     ̮     */ '\u032f', /*     ̯     */ '\u0330', /*     ̰     */
            '\u0331', /*     ̱     */ '\u0332', /*     ̲     */ '\u0333', /*     ̳     */ '\u0339', /*     ̹     */
            '\u033a', /*     ̺     */ '\u033b', /*     ̻     */ '\u033c', /*     ̼     */ '\u0345', /*     ͅ     */
            '\u0347', /*     ͇     */ '\u0348', /*     ͈     */ '\u0349', /*     ͉     */ '\u034d', /*     ͍     */
            '\u034e', /*     ͎     */ '\u0353', /*     ͓     */ '\u0354', /*     ͔     */ '\u0355', /*     ͕     */
            '\u0356', /*     ͖     */ '\u0359', /*     ͙     */ '\u035a', /*     ͚     */ '\u0323' /*     ̣     */
        };

        public static bool IsZalgoChar(char ch)
            => ch.ExistsInAny(UpChars, MidChars, DownChars);

        private static double Rand(int max) => Math.Floor(new Random().NextDouble() * max);

        public static string GenerateZalgo(string content, ZalgoIntensity intensity, IncludeChars includeChars)
            => new StringBuilder().Apply(sb => content.Where(c => !IsZalgoChar(c)).ForEach(c =>
            {
                var (up, mid, down) = intensity switch
                {
                    ZalgoIntensity.Low => (Rand(8), Rand(2), Rand(8)),
                    ZalgoIntensity.Medium => (Rand(16) / 2 + 1, Rand(6) / 2, Rand(16) / 2 + 1),
                    ZalgoIntensity.High => (Rand(64) / 4 + 3, Rand(16) / 4 + 1, Rand(64) / 4 + 3),
                    _ => throw new ArgumentException($"Invalid {nameof(ZalgoIntensity)} provided.")
                };

                sb.Append(c);
                if (includeChars.HasFlag(IncludeChars.Up))
                    for (var ind = 0; ind < up; ind++)
                        sb.Append(UpChars.GetRandomElement());
                if (includeChars.HasFlag(IncludeChars.Middle))
                    for (var ind = 0; ind < mid; ind++)
                        sb.Append(MidChars.GetRandomElement());
                if (includeChars.HasFlag(IncludeChars.Down))
                    for (var ind = 0; ind < down; ind++)
                        sb.Append(DownChars.GetRandomElement());
            })).ToString();
    }
}