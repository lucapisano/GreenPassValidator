using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DGCValidator.Services.DGC
{
    /// <summary>
    /// Vaccination proof accouring to EU EHN, version 1.0
    /// </summary>
    public partial class Vproof
    {
        /// <summary>
        /// Recovery statement
        /// </summary>
        [JsonProperty("rec", NullValueHandling = NullValueHandling.Ignore)]
        public Rec[] Rec { get; set; }

        /// <summary>
        /// Subject
        /// </summary>
        [JsonProperty("sub")]
        public Sub Sub { get; set; }

        /// <summary>
        /// Test result statement
        /// </summary>
        [JsonProperty("tst", NullValueHandling = NullValueHandling.Ignore)]
        public Tst[] Tst { get; set; }

        /// <summary>
        /// Vaccination/prophylaxis information
        /// </summary>
        [JsonProperty("vac", NullValueHandling = NullValueHandling.Ignore)]
        public Vac[] Vac { get; set; }
    }

    public partial class Rec
    {
        /// <summary>
        /// Country in which the first positive test was performed (ISO 3166 Country Code)
        /// </summary>
        [JsonProperty("cou")]
        public string Cou { get; set; }

        /// <summary>
        /// The date when the sample for the test was collected that led to a positive test result
        /// </summary>
        [JsonProperty("dat")]
        public DateTimeOffset Dat { get; set; }

        /// <summary>
        /// Disease or agent that the person has recovered from
        /// </summary>
        [JsonProperty("dis")]
        public string Dis { get; set; }
    }

    /// <summary>
    /// Subject
    /// </summary>
    public partial class Sub
    {
        /// <summary>
        /// Mandatory if no Person identifier is provided.
        /// </summary>
        [JsonProperty("dob")]
        public string Dob { get; set; }

        /// <summary>
        /// Identifiers of the vaccinated person, according to the policies applicable in each country
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public PersonIdentifier[] Id { get; set; }

        /// <summary>
        /// The legal name of the vaccinated person
        /// </summary>
        [JsonProperty("n")]
        public string N { get; set; }
    }

    public partial class PersonIdentifier
    {
        [JsonProperty("i")]
        public string I { get; set; }

        /// <summary>
        /// The type of identifier pin = personal identity number, pas = passport number, nid =
        /// national identity card number
        /// </summary>
        [JsonProperty("t")]
        public IdentifierType T { get; set; }
    }

    public partial class Tst
    {
        /// <summary>
        /// Country in which the person was tested (ISO 3166 Country Code)
        /// </summary>
        [JsonProperty("cou")]
        public string Cou { get; set; }

        /// <summary>
        /// Date when the sample for the test was collected
        /// </summary>
        [JsonProperty("dat")]
        public DateTimeOffset Dat { get; set; }

        /// <summary>
        /// Disease or agent targeted
        /// </summary>
        [JsonProperty("dis")]
        public string Dis { get; set; }

        /// <summary>
        /// Name/code of testing centre, facility or a health authority responsible for the testing
        /// event.
        /// </summary>
        [JsonProperty("fac", NullValueHandling = NullValueHandling.Ignore)]
        public string Fac { get; set; }

        /// <summary>
        /// The type of sample that was taken (e.g. nasopharyngeal swab, oropharyngeal swab, nasal
        /// swab, saliva)
        /// </summary>
        [JsonProperty("ori", NullValueHandling = NullValueHandling.Ignore)]
        public string Ori { get; set; }

        /// <summary>
        /// Negative (false) or positive (true)
        /// </summary>
        [JsonProperty("res", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Res { get; set; }

        /// <summary>
        /// Manufacturer of the RT-PCR or rapid antigen test
        /// </summary>
        [JsonProperty("tma", NullValueHandling = NullValueHandling.Ignore)]
        public string Tma { get; set; }

        /// <summary>
        /// Commercial or brand name of the RT-PCR or rapid antigen
        /// </summary>
        [JsonProperty("tna", NullValueHandling = NullValueHandling.Ignore)]
        public string Tna { get; set; }

        /// <summary>
        /// Description of the type of test that was conducted (LOINC, NPU)
        /// </summary>
        [JsonProperty("typ", NullValueHandling = NullValueHandling.Ignore)]
        public string Typ { get; set; }
    }

    public partial class Vac
    {
        /// <summary>
        /// Name/code of administering centre or a health authority responsible for the vaccination
        /// event
        /// </summary>
        [JsonProperty("adm")]
        public string Adm { get; set; }

        /// <summary>
        /// EMA's Organisations System data (SPOR).
        /// </summary>
        [JsonProperty("aut")]
        public string Aut { get; set; }

        /// <summary>
        /// The date of the vaccination event
        /// </summary>
        [JsonProperty("dat")]
        public DateTimeOffset Dat { get; set; }

        /// <summary>
        /// Generic description of the vaccine/prophylaxis or its component(s).
        /// </summary>
        [JsonProperty("des")]
        public string Des { get; set; }

        /// <summary>
        /// A distinctive combination of numbers and/or letters which specifically identifies a batch.
        /// </summary>
        [JsonProperty("lot", NullValueHandling = NullValueHandling.Ignore)]
        public string Lot { get; set; }

        /// <summary>
        /// Name of the medicinal product as registered in the country.
        /// </summary>
        [JsonProperty("nam")]
        public string Nam { get; set; }

        /// <summary>
        /// The sequence number of this dose in the series of vaccinations.
        /// </summary>
        [JsonProperty("seq")]
        public long Seq { get; set; }

        /// <summary>
        /// Disease or agent that the vaccination provides protection.against.
        /// </summary>
        [JsonProperty("tar", NullValueHandling = NullValueHandling.Ignore)]
        public string Tar { get; set; }

        /// <summary>
        /// Total number of doses in this series of vaccinations.
        /// </summary>
        [JsonProperty("tot")]
        public long Tot { get; set; }
    }

    /// <summary>
    /// The type of identifier pin = personal identity number, pas = passport number, nid =
    /// national identity card number
    /// </summary>
    public enum IdentifierType { Nid, Pas, Pin };

    public partial class Vproof
    {
        public static Vproof FromJson(string json) => JsonConvert.DeserializeObject<Vproof>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Vproof self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                IdentifierTypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class IdentifierTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(IdentifierType) || t == typeof(IdentifierType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "nid":
                    return IdentifierType.Nid;
                case "pas":
                    return IdentifierType.Pas;
                case "pin":
                    return IdentifierType.Pin;
            }
            throw new Exception("Cannot unmarshal type IdentifierType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (IdentifierType)untypedValue;
            switch (value)
            {
                case IdentifierType.Nid:
                    serializer.Serialize(writer, "nid");
                    return;
                case IdentifierType.Pas:
                    serializer.Serialize(writer, "pas");
                    return;
                case IdentifierType.Pin:
                    serializer.Serialize(writer, "pin");
                    return;
            }
            throw new Exception("Cannot marshal type IdentifierType");
        }

        public static readonly IdentifierTypeConverter Singleton = new IdentifierTypeConverter();
    }
}