using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace cyberframe
{
    public static class Serialisation
    {
        public static string Serialise<T>(this T obj, bool simplify)
        {
            var settings = simplify
                ? SerialisationConstants.WebSerializerSettings()
                : SerialisationConstants.SerialisationSettings();
            return JsonConvert.SerializeObject(obj, settings);
        }
    }

    public static class SerialisationConstants
    {
        // BEWARE - the contract resolver converts values to lowercase, that'S why the duplicity of the values
        public static readonly List<string> UnwantedFields = new List<string> {"hideFlags", "hideflags", "name"};

        public static JsonSerializerSettings SerialisationSettings()
        {
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                Formatting = Formatting.Indented,
                ContractResolver = new ShouldSerializeContractResolver()
            };
            settings.Converters.Add(new StringEnumConverter {AllowIntegerValues = false});
            return settings;
        }

        public static JsonSerializerSettings SerialisationSettings(DefaultContractResolver resolver)
        {
            var settings = SerialisationSettings();
            settings.ContractResolver = resolver;
            return settings;
        }

        public static JsonSerializerSettings WebSerializerSettings()
        {
            var settings = SerialisationSettings();
            settings.Formatting = Formatting.None;
            return settings;
        }
    }

    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        public ShouldSerializeContractResolver()
        {
            NamingStrategy = new LowercaseNamingStrategy();
        }

        public class LowercaseNamingStrategy : NamingStrategy
        {
            protected override string ResolvePropertyName(string name)
            {
                return name.ToLowerInvariant();
            }
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            properties = properties.Where(p => !SerialisationConstants.UnwantedFields.Contains(p.PropertyName))
                .ToList();
            return properties;
        }
    }
}
