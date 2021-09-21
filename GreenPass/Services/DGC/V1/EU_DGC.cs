
namespace DGCValidator.Services.DGC.V1
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// EU Digital Green Certificate
    /// </summary>
    public partial class EU_DGC
    {
        /// <summary>
        /// Date of Birth of the person addressed in the DGC. ISO 8601 date format restricted to
        /// range 1900-2099
        /// </summary>
        [JsonProperty("dob")]
        public string Dob { get; set; }

        /// <summary>
        /// Surname(s), given name(s) - in that order
        /// </summary>
        [JsonProperty("nam")]
        public Nam Nam { get; set; }

        /// <summary>
        /// Recovery Group
        /// </summary>
        [JsonProperty("r", NullValueHandling = NullValueHandling.Ignore)]
        public RecoveryElement[] Recoveries { get; set; }

        /// <summary>
        /// Test Group
        /// </summary>
        [JsonProperty("t", NullValueHandling = NullValueHandling.Ignore)]
        public TestElement[] Tests { get; set; }

        /// <summary>
        /// Vaccination Group
        /// </summary>
        [JsonProperty("v", NullValueHandling = NullValueHandling.Ignore)]
        public VaccinationElement[] Vaccinations { get; set; }

        /// <summary>
        /// Version of the schema, according to Semantic versioning (ISO, https://semver.org/ version
        /// 2.0.0 or newer)
        /// </summary>
        [JsonProperty("ver")]
        public string Ver { get; set; }
    }

    /// <summary>
    /// Surname(s), given name(s) - in that order
    ///
    /// Person name: Surname(s), given name(s) - in that order
    /// </summary>
    public partial class Nam
    {
        /// <summary>
        /// The family or primary name(s) of the person addressed in the certificate
        /// </summary>
        [JsonProperty("fn", NullValueHandling = NullValueHandling.Ignore)]
        public string Fn { get; set; }

        /// <summary>
        /// The family name(s) of the person transliterated
        /// </summary>
        [JsonProperty("fnt")]
        public string Fnt { get; set; }

        /// <summary>
        /// The given name(s) of the person addressed in the certificate
        /// </summary>
        [JsonProperty("gn", NullValueHandling = NullValueHandling.Ignore)]
        public string Gn { get; set; }

        /// <summary>
        /// The given name(s) of the person transliterated
        /// </summary>
        [JsonProperty("gnt", NullValueHandling = NullValueHandling.Ignore)]
        public string Gnt { get; set; }
    }

    /// <summary>
    /// Recovery Entry
    /// </summary>
    public partial class RecoveryElement
    {
        /// <summary>
        /// Unique Certificate Identifier, UVCI
        /// </summary>
        [JsonProperty("ci")]
        public string Ci { get; set; }

        /// <summary>
        /// Country of Test
        /// </summary>
        [JsonProperty("co")]
        public string Co { get; set; }

        /// <summary>
        /// ISO 8601 Date: Certificate Valid From
        /// </summary>
        [JsonProperty("df")]
        public DateTimeOffset Df { get; set; }

        /// <summary>
        /// Certificate Valid Until
        /// </summary>
        [JsonProperty("du")]
        public DateTimeOffset Du { get; set; }

        /// <summary>
        /// ISO 8601 Date of First Positive Test Result
        /// </summary>
        [JsonProperty("fr")]
        public DateTimeOffset Fr { get; set; }

        /// <summary>
        /// Certificate Issuer
        /// </summary>
        [JsonProperty("is")]
        public string Is { get; set; }

        [JsonProperty("tg")]
        public string Tg { get; set; }
    }

    /// <summary>
    /// Test Entry
    /// </summary>
    public partial class TestElement
    {
        /// <summary>
        /// Unique Certificate Identifier, UVCI
        /// </summary>
        [JsonProperty("ci")]
        public string Ci { get; set; }

        /// <summary>
        /// Country of Test
        /// </summary>
        [JsonProperty("co")]
        public string Co { get; set; }

        ///// <summary>
        ///// Date/Time of Test Result
        ///// </summary>
        //[JsonProperty("dr", NullValueHandling = NullValueHandling.Ignore)]
        //public DateTimeOffset? Dr { get; set; }

        /// <summary>
        /// Certificate Issuer
        /// </summary>
        [JsonProperty("is")]
        public string Is { get; set; }

        /// <summary>
        /// RAT Test name and manufacturer
        /// </summary>
        [JsonProperty("ma", NullValueHandling = NullValueHandling.Ignore)]
        public string Manufacturer { get; set; }

        /// <summary>
        /// NAA Test Name
        /// </summary>
        [JsonProperty("nm", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Date/Time of Sample Collection
        /// </summary>
        [JsonProperty("sc")]
        public DateTimeOffset SampleCollectionDate { get; set; }

        /// <summary>
        /// Testing Centre
        /// </summary>
        [JsonProperty("tc")]
        public string Tc { get; set; }

        [JsonProperty("tg")]
        public string Tg { get; set; }

        /// <summary>
        /// Test Result
        /// </summary>
        [JsonProperty("tr")]
        public string Result { get; set; }

        /// <summary>
        /// Type of Test
        /// </summary>
        [JsonProperty("tt")]
        public string TestType { get; set; }
    }

    /// <summary>
    /// Vaccination Entry
    /// </summary>
    public partial class VaccinationElement
    {
        /// <summary>
        /// Unique Certificate Identifier: UVCI
        /// </summary>
        [JsonProperty("ci")]
        public string CertificateId { get; set; }

        /// <summary>
        /// Country of Vaccination
        /// </summary>
        [JsonProperty("co")]
        public string Country { get; set; }

        /// <summary>
        /// Dose Number
        /// </summary>
        [JsonProperty("dn")]
        public long DoseNumber { get; set; }

        /// <summary>
        /// Date of Vaccination
        /// </summary>
        [JsonProperty("dt")]
        public DateTimeOffset Date { get; set; }

        /// <summary>
        /// Certificate Issuer
        /// </summary>
        [JsonProperty("is")]
        public string Issuer { get; set; }

        /// <summary>
        /// Marketing Authorization Holder - if no MAH present, then manufacturer
        /// </summary>
        [JsonProperty("ma")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// vaccine medicinal product
        /// </summary>
        [JsonProperty("mp")]
        public string MedicinalProduct { get; set; }

        /// <summary>
        /// Total Series of Doses
        /// </summary>
        [JsonProperty("sd")]
        public long SeriesOfDoses { get; set; }

        /// <summary>
        /// disease or agent targeted
        /// </summary>
        [JsonProperty("tg")]
        public string Tg { get; set; }

        /// <summary>
        /// vaccine or prophylaxis
        /// </summary>
        [JsonProperty("vp")]
        public string Vp { get; set; }
    }

    ///// <summary>
    ///// EU eHealthNetwork: Value Sets for Digital Green Certificates. version 1.0, 2021-04-16,
    ///// section 2.1
    /////
    ///// disease or agent targeted
    ///// </summary>
    //public enum Tg { The840539006 };

    ///// <summary>
    ///// RAT Test name and manufacturer
    /////
    ///// EU eHealthNetwork: Value Sets for Digital Green Certificates. version 1.0, 2021-04-16,
    ///// section 2.8
    ///// </summary>
    //public enum TMa { The1065, The1097, The1162, The1173, The1180, The1218, The1223, The1232, The1242, The1244, The1268, The1271, The1304, The1331, The1333, The1343, The1360, The1363, The1481, The1484, The1489, The1767, The344, The345 };

    ///// <summary>
    ///// Test Result
    /////
    ///// EU eHealthNetwork: Value Sets for Digital Green Certificates. version 1.0, 2021-04-16,
    ///// section 2.9
    ///// </summary>
    //public enum Tr { The260373001, The260415000 };

    ///// <summary>
    ///// Marketing Authorization Holder - if no MAH present, then manufacturer
    /////
    ///// EU eHealthNetwork: Value Sets for Digital Green Certificates. version 1.0, 2021-04-16,
    ///// section 2.4
    ///// </summary>
    //public enum VMa { BharatBiotech, GamaleyaResearchInstitute, Org100001417, Org100001699, Org100006270, Org100010771, Org100013793, Org100020693, Org100024420, Org100030215, Org100031184, Org100032020, SinovacBiotech, VectorInstitute };

    ///// <summary>
    ///// vaccine medicinal product
    /////
    ///// EU eHealthNetwork: Value Sets for Digital Green Certificates. version 1.0, 2021-04-16,
    ///// section 2.3
    ///// </summary>
    //public enum Mp { BbibpCorV, CVnCoV, Convidecia, CoronaVac, Covaxin, EpiVacCorona, Eu1201507, Eu1201525, Eu1201528, Eu1211529, InactivatedSarsCoV2VeroCell, NvxCoV2373, SputnikV };

    ///// <summary>
    ///// vaccine or prophylaxis
    /////
    ///// EU eHealthNetwork: Value Sets for Digital Green Certificates. version 1.0, 2021-04-16,
    ///// section 2.2
    ///// </summary>
    //public enum Vp { J07Bx03, The1119305005, The1119349007 };

    public partial class EU_DGC
    {
        public static EU_DGC FromJson(string json) => JsonConvert.DeserializeObject<EU_DGC>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this EU_DGC self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
/*                TgConverter.Singleton,
                TMaConverter.Singleton,
                TrConverter.Singleton,
                VMaConverter.Singleton,
                MpConverter.Singleton,
                VpConverter.Singleton,*/
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    //internal class TgConverter : JsonConverter
    //{
    //    public override bool CanConvert(Type t) => t == typeof(Tg) || t == typeof(Tg?);

    //    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    //    {
    //        if (reader.TokenType == JsonToken.Null) return null;
    //        var value = serializer.Deserialize<string>(reader);
    //        if (value == "840539006")
    //        {
    //            return Tg.The840539006;
    //        }
    //        throw new Exception("Cannot unmarshal type Tg");
    //    }

    //    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    //    {
    //        if (untypedValue == null)
    //        {
    //            serializer.Serialize(writer, null);
    //            return;
    //        }
    //        var value = (Tg)untypedValue;
    //        if (value == Tg.The840539006)
    //        {
    //            serializer.Serialize(writer, "840539006");
    //            return;
    //        }
    //        throw new Exception("Cannot marshal type Tg");
    //    }

    //    public static readonly TgConverter Singleton = new TgConverter();
    //}

    //internal class TMaConverter : JsonConverter
    //{
    //    public override bool CanConvert(Type t) => t == typeof(TMa) || t == typeof(TMa?);

    //    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    //    {
    //        if (reader.TokenType == JsonToken.Null) return null;
    //        var value = serializer.Deserialize<string>(reader);
    //        switch (value)
    //        {
    //            case "1065":
    //                return TMa.The1065;
    //            case "1097":
    //                return TMa.The1097;
    //            case "1162":
    //                return TMa.The1162;
    //            case "1173":
    //                return TMa.The1173;
    //            case "1180":
    //                return TMa.The1180;
    //            case "1218":
    //                return TMa.The1218;
    //            case "1223":
    //                return TMa.The1223;
    //            case "1232":
    //                return TMa.The1232;
    //            case "1242":
    //                return TMa.The1242;
    //            case "1244":
    //                return TMa.The1244;
    //            case "1268":
    //                return TMa.The1268;
    //            case "1271":
    //                return TMa.The1271;
    //            case "1304":
    //                return TMa.The1304;
    //            case "1331":
    //                return TMa.The1331;
    //            case "1333":
    //                return TMa.The1333;
    //            case "1343":
    //                return TMa.The1343;
    //            case "1360":
    //                return TMa.The1360;
    //            case "1363":
    //                return TMa.The1363;
    //            case "1481":
    //                return TMa.The1481;
    //            case "1484":
    //                return TMa.The1484;
    //            case "1489":
    //                return TMa.The1489;
    //            case "1767":
    //                return TMa.The1767;
    //            case "344":
    //                return TMa.The344;
    //            case "345":
    //                return TMa.The345;
    //        }
    //        throw new Exception("Cannot unmarshal type TMa");
    //    }

    //    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    //    {
    //        if (untypedValue == null)
    //        {
    //            serializer.Serialize(writer, null);
    //            return;
    //        }
    //        var value = (TMa)untypedValue;
    //        switch (value)
    //        {
    //            case TMa.The1065:
    //                serializer.Serialize(writer, "1065");
    //                return;
    //            case TMa.The1097:
    //                serializer.Serialize(writer, "1097");
    //                return;
    //            case TMa.The1162:
    //                serializer.Serialize(writer, "1162");
    //                return;
    //            case TMa.The1173:
    //                serializer.Serialize(writer, "1173");
    //                return;
    //            case TMa.The1180:
    //                serializer.Serialize(writer, "1180");
    //                return;
    //            case TMa.The1218:
    //                serializer.Serialize(writer, "1218");
    //                return;
    //            case TMa.The1223:
    //                serializer.Serialize(writer, "1223");
    //                return;
    //            case TMa.The1232:
    //                serializer.Serialize(writer, "1232");
    //                return;
    //            case TMa.The1242:
    //                serializer.Serialize(writer, "1242");
    //                return;
    //            case TMa.The1244:
    //                serializer.Serialize(writer, "1244");
    //                return;
    //            case TMa.The1268:
    //                serializer.Serialize(writer, "1268");
    //                return;
    //            case TMa.The1271:
    //                serializer.Serialize(writer, "1271");
    //                return;
    //            case TMa.The1304:
    //                serializer.Serialize(writer, "1304");
    //                return;
    //            case TMa.The1331:
    //                serializer.Serialize(writer, "1331");
    //                return;
    //            case TMa.The1333:
    //                serializer.Serialize(writer, "1333");
    //                return;
    //            case TMa.The1343:
    //                serializer.Serialize(writer, "1343");
    //                return;
    //            case TMa.The1360:
    //                serializer.Serialize(writer, "1360");
    //                return;
    //            case TMa.The1363:
    //                serializer.Serialize(writer, "1363");
    //                return;
    //            case TMa.The1481:
    //                serializer.Serialize(writer, "1481");
    //                return;
    //            case TMa.The1484:
    //                serializer.Serialize(writer, "1484");
    //                return;
    //            case TMa.The1489:
    //                serializer.Serialize(writer, "1489");
    //                return;
    //            case TMa.The1767:
    //                serializer.Serialize(writer, "1767");
    //                return;
    //            case TMa.The344:
    //                serializer.Serialize(writer, "344");
    //                return;
    //            case TMa.The345:
    //                serializer.Serialize(writer, "345");
    //                return;
    //        }
    //        throw new Exception("Cannot marshal type TMa");
    //    }

    //    public static readonly TMaConverter Singleton = new TMaConverter();
    //}

    //internal class TrConverter : JsonConverter
    //{
    //    public override bool CanConvert(Type t) => t == typeof(Tr) || t == typeof(Tr?);

    //    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    //    {
    //        if (reader.TokenType == JsonToken.Null) return null;
    //        var value = serializer.Deserialize<string>(reader);
    //        switch (value)
    //        {
    //            case "260373001":
    //                return Tr.The260373001;
    //            case "260415000":
    //                return Tr.The260415000;
    //        }
    //        throw new Exception("Cannot unmarshal type Tr");
    //    }

    //    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    //    {
    //        if (untypedValue == null)
    //        {
    //            serializer.Serialize(writer, null);
    //            return;
    //        }
    //        var value = (Tr)untypedValue;
    //        switch (value)
    //        {
    //            case Tr.The260373001:
    //                serializer.Serialize(writer, "260373001");
    //                return;
    //            case Tr.The260415000:
    //                serializer.Serialize(writer, "260415000");
    //                return;
    //        }
    //        throw new Exception("Cannot marshal type Tr");
    //    }

    //    public static readonly TrConverter Singleton = new TrConverter();
    //}

    //internal class VMaConverter : JsonConverter
    //{
    //    public override bool CanConvert(Type t) => t == typeof(VMa) || t == typeof(VMa?);

    //    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    //    {
    //        if (reader.TokenType == JsonToken.Null) return null;
    //        var value = serializer.Deserialize<string>(reader);
    //        switch (value)
    //        {
    //            case "Bharat-Biotech":
    //                return VMa.BharatBiotech;
    //            case "Gamaleya-Research-Institute":
    //                return VMa.GamaleyaResearchInstitute;
    //            case "ORG-100001417":
    //                return VMa.Org100001417;
    //            case "ORG-100001699":
    //                return VMa.Org100001699;
    //            case "ORG-100006270":
    //                return VMa.Org100006270;
    //            case "ORG-100010771":
    //                return VMa.Org100010771;
    //            case "ORG-100013793":
    //                return VMa.Org100013793;
    //            case "ORG-100020693":
    //                return VMa.Org100020693;
    //            case "ORG-100024420":
    //                return VMa.Org100024420;
    //            case "ORG-100030215":
    //                return VMa.Org100030215;
    //            case "ORG-100031184":
    //                return VMa.Org100031184;
    //            case "ORG-100032020":
    //                return VMa.Org100032020;
    //            case "Sinovac-Biotech":
    //                return VMa.SinovacBiotech;
    //            case "Vector-Institute":
    //                return VMa.VectorInstitute;
    //        }
    //        throw new Exception("Cannot unmarshal type VMa");
    //    }

    //    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    //    {
    //        if (untypedValue == null)
    //        {
    //            serializer.Serialize(writer, null);
    //            return;
    //        }
    //        var value = (VMa)untypedValue;
    //        switch (value)
    //        {
    //            case VMa.BharatBiotech:
    //                serializer.Serialize(writer, "Bharat-Biotech");
    //                return;
    //            case VMa.GamaleyaResearchInstitute:
    //                serializer.Serialize(writer, "Gamaleya-Research-Institute");
    //                return;
    //            case VMa.Org100001417:
    //                serializer.Serialize(writer, "ORG-100001417");
    //                return;
    //            case VMa.Org100001699:
    //                serializer.Serialize(writer, "ORG-100001699");
    //                return;
    //            case VMa.Org100006270:
    //                serializer.Serialize(writer, "ORG-100006270");
    //                return;
    //            case VMa.Org100010771:
    //                serializer.Serialize(writer, "ORG-100010771");
    //                return;
    //            case VMa.Org100013793:
    //                serializer.Serialize(writer, "ORG-100013793");
    //                return;
    //            case VMa.Org100020693:
    //                serializer.Serialize(writer, "ORG-100020693");
    //                return;
    //            case VMa.Org100024420:
    //                serializer.Serialize(writer, "ORG-100024420");
    //                return;
    //            case VMa.Org100030215:
    //                serializer.Serialize(writer, "ORG-100030215");
    //                return;
    //            case VMa.Org100031184:
    //                serializer.Serialize(writer, "ORG-100031184");
    //                return;
    //            case VMa.Org100032020:
    //                serializer.Serialize(writer, "ORG-100032020");
    //                return;
    //            case VMa.SinovacBiotech:
    //                serializer.Serialize(writer, "Sinovac-Biotech");
    //                return;
    //            case VMa.VectorInstitute:
    //                serializer.Serialize(writer, "Vector-Institute");
    //                return;
    //        }
    //        throw new Exception("Cannot marshal type VMa");
    //    }

    //    public static readonly VMaConverter Singleton = new VMaConverter();
    //}

    //internal class MpConverter : JsonConverter
    //{
    //    public override bool CanConvert(Type t) => t == typeof(Mp) || t == typeof(Mp?);

    //    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    //    {
    //        if (reader.TokenType == JsonToken.Null) return null;
    //        var value = serializer.Deserialize<string>(reader);
    //        switch (value)
    //        {
    //            case "BBIBP-CorV":
    //                return Mp.BbibpCorV;
    //            case "CVnCoV":
    //                return Mp.CVnCoV;
    //            case "Convidecia":
    //                return Mp.Convidecia;
    //            case "CoronaVac":
    //                return Mp.CoronaVac;
    //            case "Covaxin":
    //                return Mp.Covaxin;
    //            case "EU/1/20/1507":
    //                return Mp.Eu1201507;
    //            case "EU/1/20/1525":
    //                return Mp.Eu1201525;
    //            case "EU/1/20/1528":
    //                return Mp.Eu1201528;
    //            case "EU/1/21/1529":
    //                return Mp.Eu1211529;
    //            case "EpiVacCorona":
    //                return Mp.EpiVacCorona;
    //            case "Inactivated-SARS-CoV-2-Vero-Cell":
    //                return Mp.InactivatedSarsCoV2VeroCell;
    //            case "NVX-CoV2373":
    //                return Mp.NvxCoV2373;
    //            case "Sputnik-V":
    //                return Mp.SputnikV;
    //        }
    //        throw new Exception("Cannot unmarshal type Mp");
    //    }

    //    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    //    {
    //        if (untypedValue == null)
    //        {
    //            serializer.Serialize(writer, null);
    //            return;
    //        }
    //        var value = (Mp)untypedValue;
    //        switch (value)
    //        {
    //            case Mp.BbibpCorV:
    //                serializer.Serialize(writer, "BBIBP-CorV");
    //                return;
    //            case Mp.CVnCoV:
    //                serializer.Serialize(writer, "CVnCoV");
    //                return;
    //            case Mp.Convidecia:
    //                serializer.Serialize(writer, "Convidecia");
    //                return;
    //            case Mp.CoronaVac:
    //                serializer.Serialize(writer, "CoronaVac");
    //                return;
    //            case Mp.Covaxin:
    //                serializer.Serialize(writer, "Covaxin");
    //                return;
    //            case Mp.Eu1201507:
    //                serializer.Serialize(writer, "EU/1/20/1507");
    //                return;
    //            case Mp.Eu1201525:
    //                serializer.Serialize(writer, "EU/1/20/1525");
    //                return;
    //            case Mp.Eu1201528:
    //                serializer.Serialize(writer, "EU/1/20/1528");
    //                return;
    //            case Mp.Eu1211529:
    //                serializer.Serialize(writer, "EU/1/21/1529");
    //                return;
    //            case Mp.EpiVacCorona:
    //                serializer.Serialize(writer, "EpiVacCorona");
    //                return;
    //            case Mp.InactivatedSarsCoV2VeroCell:
    //                serializer.Serialize(writer, "Inactivated-SARS-CoV-2-Vero-Cell");
    //                return;
    //            case Mp.NvxCoV2373:
    //                serializer.Serialize(writer, "NVX-CoV2373");
    //                return;
    //            case Mp.SputnikV:
    //                serializer.Serialize(writer, "Sputnik-V");
    //                return;
    //        }
    //        throw new Exception("Cannot marshal type Mp");
    //    }

    //    public static readonly MpConverter Singleton = new MpConverter();
    //}

    //internal class VpConverter : JsonConverter
    //{
    //    public override bool CanConvert(Type t) => t == typeof(Vp) || t == typeof(Vp?);

    //    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    //    {
    //        if (reader.TokenType == JsonToken.Null) return null;
    //        var value = serializer.Deserialize<string>(reader);
    //        switch (value)
    //        {
    //            case "1119305005":
    //                return Vp.The1119305005;
    //            case "1119349007":
    //                return Vp.The1119349007;
    //            case "J07BX03":
    //                return Vp.J07Bx03;
    //        }
    //        throw new Exception("Cannot unmarshal type Vp");
    //    }

    //    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    //    {
    //        if (untypedValue == null)
    //        {
    //            serializer.Serialize(writer, null);
    //            return;
    //        }
    //        var value = (Vp)untypedValue;
    //        switch (value)
    //        {
    //            case Vp.The1119305005:
    //                serializer.Serialize(writer, "1119305005");
    //                return;
    //            case Vp.The1119349007:
    //                serializer.Serialize(writer, "1119349007");
    //                return;
    //            case Vp.J07Bx03:
    //                serializer.Serialize(writer, "J07BX03");
    //                return;
    //        }
    //        throw new Exception("Cannot marshal type Vp");
    //    }

    //    public static readonly VpConverter Singleton = new VpConverter();
    //}
}
