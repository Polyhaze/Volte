using System.IO;
using System.Runtime.CompilerServices;

namespace Gommon;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum HexNumberFormat
{
    RGB,
    ARGB,
    RGBA
}

/// <summary>
///     Extensions for any class in the System namespace, including sub-namespaces, such as System.Text.
/// </summary>
public static partial class Extensions
{
    public static string ToHexadecimalString(this System.Drawing.Color color, 
        HexNumberFormat numberFormat = HexNumberFormat.RGB, 
        string prefix = "#")
    {
        prefix ??= string.Empty;

        return (numberFormat switch
        {
            HexNumberFormat.ARGB => color.A.ToString("X2") + rgb(),
            HexNumberFormat.RGBA => rgb() + color.A.ToString("X2"),
            _ => rgb()
        }).Prepend(prefix);


        string rgb() => $"{color.R:X2}{color.G:X2}{color.B:X2}";
    }
    
    public static string FormatSignatureString(this MethodInfo method) 
        => method.FormatSignatureString(method.GetParameters().Select(x => x.ParameterType));

    public static string FormatSignatureString(this MethodInfo method, params IEnumerable<Type> argTypes)
    {
        var sb = new StringBuilder();
        sb.Append(method.ReturnType.AsPrettyString());
        sb.Append(' ');
        sb.Append(method.Name);
        sb.Append('(');

        sb.Append(argTypes
            .Select(x => x.AsPrettyString())
            .JoinToString(", ")
        );
            
        sb.Append(')');

        return sb.ToString();
    }
    
    public static bool ExistsInAny<T>(this T @this, params IEnumerable<T>[] collections) 
        => collections.Any(x => x.Contains(@this));

    public static string CalculateUptime(this Process process)
        => (DateTime.Now - process.StartTime).Humanize(3);

    public static Task<IUserMessage> SendFileToAsync(this Stream stream,
        ITextChannel channel, string filename, string text = null, bool isTts = false, Embed embed = null,
        RequestOptions options = null,
        bool isSpoiler = false, AllowedMentions allowedMentions = null, MessageReference reference = null)
        => channel.SendFileAsync(stream, filename, text, isTts, embed, options, isSpoiler, allowedMentions,
            reference);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Truncate(this string value, int limit, Func<int, string> truncationString)
    {
        var content = value[..(limit+1)];
        var truncatedCharacters = value.Length - content.Length;
        if (truncatedCharacters > 0)
            content += truncationString(truncatedCharacters);

        return content;
    }
    
    #nullable enable
    
    public static void SentryCapture(this Exception? e, Action<Scope>? configureScope = null)
    {
        if (e is null or TaskCanceledException or OperationCanceledException) return;
        
        if (configureScope != null) 
            SentrySdk.CaptureException(e, configureScope);
        else SentrySdk.CaptureException(e);
    }
    
    public static bool TryParse<T>(this string? s, [MaybeNullWhen(false)] out T result, IFormatProvider? formatProvider = null) where T : IParsable<T>
        => T.TryParse(s, formatProvider, out result);
    
    public static T Parse<T>(this string s, IFormatProvider? formatProvider = null) where T : IParsable<T>
        => T.Parse(s, formatProvider);
    
    public static bool TryParse<T>(this ReadOnlySpan<char> s, [MaybeNullWhen(false)] out T result, IFormatProvider? formatProvider = null) where T : ISpanParsable<T>
        => T.TryParse(s, formatProvider, out result);
    
    public static T Parse<T>(this ReadOnlySpan<char> s, IFormatProvider? formatProvider = null) where T : ISpanParsable<T>
        => T.Parse(s, formatProvider);
}