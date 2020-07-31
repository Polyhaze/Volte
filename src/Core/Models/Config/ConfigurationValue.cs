using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.ComponentModel;

using YamlDotNet.Serialization;

namespace BrackeysBot
{
    public class ConfigurationValue
    {
        public string Name { get; }
        public string Description { get; }

        public bool IsArray => _property.PropertyType.IsArray;

        private PropertyInfo _property;
        private object _instance;

        public ConfigurationValue(PropertyInfo property, object instance)
        {
            _property = property;
            _instance = instance;

            Name = property.GetCustomAttribute<YamlMemberAttribute>()?.Alias ?? property.Name;
            Description = property.GetCustomAttribute<DescriptionAttribute>()?.Description;
        }

        public object GetValue()
            => _property.GetValue(_instance);
        public bool SetValue(object value)
        {
            Type source = value.GetType();
            Type target = _property.PropertyType;

            if (target.IsClass && (value == null || (value is string s && s == "null")))
            {
                _property.SetValue(_instance, null);
                return true;
            }

            object convertedValue = value;
            if (source != target)
            {
                if (!target.IsArray)
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(target);

                    if (converter != null && converter.CanConvertFrom(source) && converter.IsValid(value))
                        convertedValue = converter.ConvertFrom(value);
                    else
                        return false;
                }
                else
                {
                    if (source == typeof(string))
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(target.GetElementType());

                        if (converter == null || !converter.CanConvertFrom(source))
                            return false;

                        Array convertedValueArray = value.ToString().Split(',')
                            .Select(part => converter.ConvertFrom(part.Trim()))
                            .ToArray();

                        Array typedConvertedArray = Array.CreateInstance(target.GetElementType(), convertedValueArray.Length);
                        Array.Copy(convertedValueArray, typedConvertedArray, convertedValueArray.Length);

                        convertedValue = typedConvertedArray;
                    }
                    else
                        return false;
                }
            }

            _property.SetValue(_instance, convertedValue);
            return true;
        }

        public bool HasAttribute<T>() where T : Attribute
            => _property.GetCustomAttribute<T>() != null;

        public override string ToString()
        {
            object value = GetValue();
            if (value == null)
                return "null";

            string representation = IsArray
                ? FormatArrayValue(value)
                : FormatValueByDisplayAttribute(value.ToString());

            return FormatValueByShortenAttribute(representation);
        }

        private string FormatArrayValue(object value)
        {
            if (value == null)
                return "null";

            string[] entries = ((IEnumerable)value).OfType<object>()
                .Select(o => FormatValueByDisplayAttribute(o.ToString()))
                .ToArray();

            return string.Join(", ", entries);
        }
        private string FormatValueByDisplayAttribute(string value)
        {
            ConfigDisplayAttribute configDisplay = _property.GetCustomAttribute<ConfigDisplayAttribute>();
            return configDisplay != null
                ? configDisplay.FormatValue(value)
                : value;
        }
        private string FormatValueByShortenAttribute(string value)
        {
            ShortenAttribute shorten = _property.GetCustomAttribute<ShortenAttribute>();
            if (shorten != null && value.Length > shorten.Length)
                value = value.Substring(0, shorten.Length) + "[...]";
            return value;
        }
    }
}
