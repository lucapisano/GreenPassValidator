namespace DGCValidator.Services.DGC.ValueSet
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class ValueSet
    {
        [JsonProperty("valueSetId")]
        public string ValueSetId { get; set; }

        [JsonProperty("valueSetDate")]
        public DateTimeOffset ValueSetDate { get; set; }

        [JsonProperty("valueSetValues")]
        public Dictionary<string,ValueSetValue> ValueSetValues { get; set; }
    }

    public partial class ValueSet
    {
        public static ValueSet FromJson(string json) => JsonConvert.DeserializeObject<ValueSet>(json, ValueSetConverter.Settings);
    }

    public static class ValueSetSerialize
    {
        public static string ToJson(this ValueSet self) => JsonConvert.SerializeObject(self, ValueSetConverter.Settings);
    }

    internal static class ValueSetConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    public partial class ValueSetValue
    {
        [JsonProperty("display")]
        public string Display { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("system")]
        public string System { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

    }

    public partial class ValueSetValue
    {
        public static Dictionary<string, ValueSetValue> FromJson(string json) => JsonConvert.DeserializeObject<Dictionary<string, ValueSetValue>>(json, ValueSetValueConverter.Settings);
    }

    public static class ValueSetValueSerialize
    {
        public static string ToJson(this Dictionary<string, ValueSetValue> self) => JsonConvert.SerializeObject(self, ValueSetValueConverter.Settings);
    }

    internal static class ValueSetValueConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

}
