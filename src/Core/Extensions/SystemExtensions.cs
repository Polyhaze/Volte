using System.Text;

namespace Gommon
{
    /// <summary>
    ///     Extensions for any class in the System namespace, including sub-namespaces, such as System.Text.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        ///     Appends the given line to the current <see cref="StringBuilder"/> if the given <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="sb">The current <see cref="StringBuilder"/>.</param>
        /// <param name="line">The line to append.</param>
        /// <param name="condition">The condition to test against.</param>
        /// <returns><see cref="StringBuilder"/> with the line appended or not.</returns>
        public static StringBuilder AppendLineIf(this StringBuilder sb, string line, bool condition)
        {
            if (condition)
            {
                sb.AppendLine(line);
                return sb;
            }
            return sb;
        }
        /// <summary>
        ///     Appends the given text to the current <see cref="StringBuilder"/> if the given <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="sb">The current <see cref="StringBuilder"/>.</param>
        /// <param name="text">The text to append.</param>
        /// <param name="condition">The condition to test against.</param>
        /// <returns><see cref="StringBuilder"/> with the current line appended or not.</returns>
        public static StringBuilder AppendIf(this StringBuilder sb, string text, bool condition)
        {
            if (condition)
            {
                sb.Append(text);
                return sb;
            }
            return sb;
        }
    }
}
