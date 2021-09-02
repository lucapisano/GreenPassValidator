using System;
using System.Collections.Generic;

using System.Globalization;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/**
* A JWKS representation class.
*
* @author Henrik Bengtsson (henrik@sondaica.se)
* @author Martin Lindström (martin@idsec.se)
* @author Henric Norlander (extern.henric.norlander@digg.se)
*/
namespace DGCValidator.Services.CWT.Certificates
{
    public partial class Jwks
    {
        [JsonProperty("keys")]
        public Key[] Keys { get; set; }
    }

    public partial class Key
    {
        [JsonProperty("crv")]
        public string Crv { get; set; }

        [JsonProperty("kid")]
        public string Kid { get; set; }

        [JsonProperty("kty")]
        public string Kty { get; set; }

        [JsonProperty("x")]
        public string X { get; set; }

        [JsonProperty("x5a")]
        public X5A X5A { get; set; }

        [JsonProperty("x5t#S256")]
        public string X5TS256 { get; set; }

        [JsonProperty("y")]
        public string Y { get; set; }

        [JsonProperty("n")]
        public string N { get; set; }

        [JsonProperty("e")]
        public string E { get; set; }
    }

    public partial class X5A
    {
        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        [JsonProperty("serial")]
        public BigInteger Serial { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }
    }

    public partial class Jwks
    {
        public static Jwks FromJson(string json) => JsonConvert.DeserializeObject<Jwks>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Jwks self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
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
