using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;
using System.Linq;

namespace BrackeysBot.Core.Models
{
    public class CustomCommandFeatureConverter : JsonConverter<CustomCommandFeature>
    {
        private const string _featureTypeProperty = "FeatureType";
        private const string _featureValueProperty = "FeatureValue";

        public override bool CanConvert(Type type)
        {
            return typeof(CustomCommandFeature).IsAssignableFrom(type);
        }

        public override CustomCommandFeature Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            if (!reader.Read()
                    || reader.TokenType != JsonTokenType.PropertyName
                    || reader.GetString() != _featureTypeProperty)
            {
                throw new JsonException();
            }
            if (!reader.Read() || reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            CustomCommandFeature baseClass;
            string typeName = reader.GetString();

            Type featureType = typeof(CustomCommandFeature).Assembly.GetTypes()
                .FirstOrDefault(t => t.Name.Equals(typeName));

            if (featureType == null)
                throw new NotSupportedException();

            if (!reader.Read() || reader.GetString() != _featureValueProperty)
                throw new JsonException();

            baseClass = JsonSerializer.Deserialize(ref reader, featureType) as CustomCommandFeature;

            //switch (typeDiscriminator)
            //{
            //    case TypeDiscriminator.DerivedA:
            //        if (!reader.Read() || reader.GetString() != "TypeValue")
            //        {
            //            throw new JsonException();
            //        }
            //        if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            //        {
            //            throw new JsonException();
            //        }
            //        baseClass = (DerivedA)JsonSerializer.Deserialize(ref reader, typeof(DerivedA));
            //        break;
            //    case TypeDiscriminator.DerivedB:
            //        if (!reader.Read() || reader.GetString() != "TypeValue")
            //        {
            //            throw new JsonException();
            //        }
            //        if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            //        {
            //            throw new JsonException();
            //        }
            //        baseClass = (DerivedB)JsonSerializer.Deserialize(ref reader, typeof(DerivedB));
            //        break;
            //    default:
            //        throw new NotSupportedException();
            //}

            if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
                throw new JsonException();

            return baseClass;
        }

        public override void Write(Utf8JsonWriter writer, CustomCommandFeature value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            Type featureType = value.GetType();

            if (!CanConvert(featureType))
                throw new NotSupportedException();

            writer.WriteString(_featureTypeProperty, featureType.Name);
            writer.WritePropertyName(_featureValueProperty);
            JsonSerializer.Serialize(writer, value, featureType);

            //if (value is DerivedA derivedA)
            //{
            //    writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.DerivedA);
            //    writer.WritePropertyName("TypeValue");
            //    JsonSerializer.Serialize(writer, derivedA);
            //}
            //else if (value is DerivedB derivedB)
            //{
            //    writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.DerivedB);
            //    writer.WritePropertyName("TypeValue");
            //    JsonSerializer.Serialize(writer, derivedB);
            //}
            //else
            //{
            //    throw new NotSupportedException();
            //}

            writer.WriteEndObject();
        }
    }
}