using System.Globalization;
using System.Text.Json.Serialization;

namespace Volte.Entities;

public readonly record struct Snowflake(ulong Raw) : IComparable<Snowflake>, IComparable, IParsable<Snowflake>
{
    public int CompareTo(Snowflake other) => Raw.CompareTo(other.Raw);

    public int CompareTo(object obj)
    {
        if (obj is not Snowflake other)
            throw new ArgumentException("Argument to Snowflake#CompareTo must be another Snowflake.", nameof(obj));

        return CompareTo(other);
    }

    public static bool operator >(Snowflake left, Snowflake right) => left.CompareTo(right) > 0;

    public static bool operator <(Snowflake left, Snowflake right) => left.CompareTo(right) < 0;

    public DateTimeOffset Date => SnowflakeUtils.FromSnowflake(Raw);

    public string ToString(bool fullInfo) => fullInfo ? ToString() : Raw.ToString();
    public static implicit operator Snowflake(ulong raw) => new(raw);
    public static implicit operator ulong(Snowflake snowflake) => snowflake.Raw;
    public static implicit operator Snowflake(DateTimeOffset dto) => FromDate(dto);
    public static implicit operator DateTimeOffset(Snowflake snowflake) => snowflake.Date;
    public static implicit operator Snowflake(DateTime dto) => FromDate(dto);
    public static implicit operator DateTime(Snowflake snowflake) => snowflake.Date.DateTime;
    
    public static Snowflake Zero { get; set; } = default;
    
    public static Snowflake FromDate(DateTimeOffset dto) => new(SnowflakeUtils.ToSnowflake(dto));

    public static Snowflake? TryParse(string value, IFormatProvider formatProvider = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        // As number
        if (ulong.TryParse(value, NumberStyles.None, formatProvider, out var number))
            return new Snowflake(number);

        // As date
        if (DateTimeOffset.TryParse(value, formatProvider, DateTimeStyles.None, out var instant))
            return FromDate(instant);

        return null;
    }

    public static Snowflake Parse(string s, IFormatProvider provider) 
        => TryParse(s, provider)
           ?? throw new ArgumentException($"'{s}' did not contain a valid 64-bit unsigned integer or DateTimeOffset.");

    public static bool TryParse(string s, IFormatProvider provider, out Snowflake result)
    {
        var res = TryParse(s, provider);
        result = res ?? Zero;

        return res.HasValue;
    }
    
    public static bool TryParse(string s, out Snowflake result)
    {
        var res = TryParse(s);
        result = res ?? Zero;

        return res.HasValue;
    }

    public class JsonConverter : JsonConverter<Snowflake>
    {
        public override Snowflake Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return Parse(reader.GetString()!, null);
                case JsonTokenType.Number:
                    return new Snowflake(reader.GetUInt64());
                default:
                    throw new InvalidOperationException("Snowflake JSON value is expected to be a datetime/integer string or a raw integer. Prefer raw integers.");
            }
        }

        public override void Write(Utf8JsonWriter writer, Snowflake value, JsonSerializerOptions options) 
            => writer.WriteNumberValue(value.Raw);
    }
}