using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace BrackeysBot.Services
{
    public sealed class ConfigurationService : BrackeysBotService
    {
        private DataService _data;
        private BotConfiguration _config;

        private static readonly HashSet<Type> _exposedPropertyTypes
            = new HashSet<Type>() { typeof(string), typeof(bool), 
                typeof(int), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong),
                typeof(float), typeof(double) };
        
        public ConfigurationService(DataService data)
        {
            _data = data;
            _config = data.Configuration;
        }

        public bool TryGetValue(string name, out ConfigurationValue value)
        {
            value = GetConfigurationValues().FirstOrDefault(v => v.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            return value != null;
        }
        public IEnumerable<ConfigurationValue> GetConfigurationValues()
            => GetConfigProperties().Select(p => new ConfigurationValue(p, _config));
        public void Save()
            => _data.SaveConfiguration();

        private IEnumerable<PropertyInfo> GetConfigProperties()
            => typeof(BotConfiguration).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty)
                .Where(p =>
                {
                    bool isValidType = _exposedPropertyTypes.Contains(p.PropertyType)
                        || p.PropertyType.IsArray && _exposedPropertyTypes.Contains(p.PropertyType.GetElementType());
                    bool confidential = p.GetCustomAttribute<ConfidentialAttribute>() != null;

                    return isValidType && !confidential;
                });
    }
}
