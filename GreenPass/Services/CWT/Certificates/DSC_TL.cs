namespace DGCValidator.Services.CWT.Certificates
{
    /**
 * 
 * @author Henrik Bengtsson (henrik@sondaica.se)
 * @author Martin Lindström (martin@idsec.se)
 * @author Henric Norlander (extern.henric.norlander@digg.se)
 */

    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class DSC_TL_HEADER
    {
        [JsonProperty("typ")]
        public string Typ { get; set; }

        [JsonProperty("alg")]
        public string Alg { get; set; }

        [JsonProperty("x5c")]
        public string[] X5C { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("aud")]
        public string Aud { get; set; }
    }

    public partial class DSC_TL_HEADER
    {
        public static DSC_TL_HEADER FromJson(string json) => JsonConvert.DeserializeObject<DSC_TL_HEADER>(json);
    }


    /// <summary>
    /// Schema defining the payload format for Document Signing Certificate - Trust List
    /// information
    /// </summary>
    public partial class DSC_TL
    {
        /// <summary>
        /// Optional array of identifiers of the audiences
        /// </summary>
        [JsonProperty("aud", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Aud { get; set; }

        /// <summary>
        /// List of trusted DSC for each country where the country code (ISO 3166-1 alpha-2) is the
        /// key
        /// </summary>
        [JsonProperty("dsc_trust_list")]
        public Dictionary<string, DscTrust> DscTrustList { get; set; }

        /// <summary>
        /// Expiration time (seconds since epoch)
        /// </summary>
        [JsonProperty("exp")]
        public long Exp { get; set; }

        /// <summary>
        /// Issued at time (seconds since epoch)
        /// </summary>
        [JsonProperty("iat")]
        public long Iat { get; set; }

        /// <summary>
        /// Identifier of DSC-TL
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        /// <summary>
        /// Identifier of the issuer of the DSC-TL
        /// </summary>
        [JsonProperty("iss")]
        public string Iss { get; set; }
    }

    public partial class DSC_TL
    {
        public static DSC_TL FromJson(string json) => JsonConvert.DeserializeObject<DSC_TL>(json, DSC_TLConverter.Settings);
    }

    public partial class DscTrust
    {
        /// <summary>
        /// List of eku
        /// 
        /// </summary>
        [JsonProperty("eku")]
        public Dictionary<string, object> Eku { get; set; }

        /// <summary>
        /// Jwk keys
        /// </summary>
        [JsonProperty("keys")]
        public Key[] Keys { get; set; }

    }

    public static class DSC_TLSerialize
    {
        public static string ToJson(this DSC_TL self) => JsonConvert.SerializeObject(self, DSC_TLConverter.Settings);
    }

    internal static class DSC_TLConverter
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
