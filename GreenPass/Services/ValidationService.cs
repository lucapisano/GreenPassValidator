using System;
using System.IO;
using System.Text;
using DGCValidator.Services.CWT;
using DGCValidator.Services;
using DGCValidator.Services.DGC;
using DGCValidator.Services.DGC.V1;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using PeterO.Cbor;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using GreenPass.Services;
using System.Linq;
using System.Collections.Generic;
using GreenPass.Models;
using Microsoft.Extensions.Logging;

namespace GreenPass
{
    /**
     * @author Luca Pisano (luca@lucapisano.it)
     * @author Henrik Bengtsson (henrik@sondaica.se)
     * @author Martin Lindström (martin@idsec.se)
     * @author Henric Norlander (extern.henric.norlander@digg.se)
     */
    public class ValidationService
    {
        /*
         * UPDATE 17-01-2022: added different validation types in order to enforce Italian DPCM specifications.
         * Reference: https://github.com/ministero-salute/it-dgc-documentation/blob/master/SCANMODE.md 
         * There are three different validation types:
         * 3G is the base validation, it works as usual;
         * 2G is the strengthened validation: if the DGC presented contains just a test, it is invalid. If it is a recovery or a vaccine, then it is valid;
         * Booster validation is a validation which returns valid only if the DGC contains a complete vaccination cycle and a booster dose. If the DGC contains just a completed vaccination cycle or a recovery, it will ask the user to show a valid negative Covid-19 test.
         * 
         * The validation results become three:
         * VALID: the DGC is valid.
         * INVALID: the DGC is invalid.
         * TEST_NEEDED: the DGC is valid, but it must be shown with a valid Covid-19 test.
         */

        /* 
         * !!!IMPORTANT!!! 
         * There is a borderline case regarding the validation for cross vaccinations using Johnson & Johnson vaccines. 
         * Since the dn/sd ratio was not standardised at first, some EU States utilised a different approach from others to classify heterogeneous (cross) vaccinations. 
         * In particular, a J&J vaccination completed with a booster dose of another medicinal product may be classified as a 2/2 dn/sd ratio, instead of 2/1 or 3/3, yielding a TEST_NEEDED result instead of a VALID one.
         * This library DOES NOT implement this case, since the issue is being solved by the EU commission by standardising cross vaccinations         .
         * See reference https://ec.europa.eu/commission/presscorner/detail/en/ip_21_6837 for further details.
         */

        private readonly CertificateManager _certManager;
        private CachingService _cachingService;
        private ILogger<ValidationService> _logger;

        public enum ValidationType
        {
            ThreeG, // 3G (vaccine OR recovery OR test): base validation
            TwoG,   // 2G (vaccine OR recovery): reinforced validation
            Booster // Booster validation : only vaccines with a completed cycle AND a booster dose are valid
        };

        public ValidationService(CertificateManager certificateManager, IServiceProvider sp)
        {
            _certManager = certificateManager;
            _cachingService = sp.GetRequiredService<CachingService>();
            _logger = sp.GetService<ILogger<ValidationService>>();
        }

        public async Task<SignedDGC> Validate(String codeData, ValidationType validationMode = ValidationType.ThreeG)
        {
            try
            {
                // The base45 encoded data should begin with HC1
                if (codeData.StartsWith("HC1:"))
                {
                    string base45CodedData = codeData.Substring(4);

                    // Base 45 decode data
                    byte[] base45DecodedData = Base45Decoding(Encoding.GetEncoding("UTF-8").GetBytes(base45CodedData));

                    // zlib decompression
                    byte[] uncompressedData = ZlibDecompression(base45DecodedData);

                    SignedDGC vacProof = new SignedDGC();
                    // Sign and encrypt data
                    byte[] signedData = await VerifySignedData(uncompressedData, vacProof, _certManager, _logger);

                    // Get json from CBOR representation of ProofCode
                    EU_DGC eU_DGC = GetVaccinationProofFromCbor(signedData);
                    vacProof.Dgc = eU_DGC;
                    
                    // apply national rules to validate
                    await ApplyRules(vacProof);


                    // check for different validation types

                    // if validationMode == ThreeG (3G), it is a base validation
                    if (validationMode == ValidationType.ThreeG)
                    {
                        return vacProof;
                    }

                    // TwoG (2G): "reinforced" validation, tests become invalid while vaccinations & recoveries stay valid
                    else if (validationMode == ValidationType.TwoG)
                    {
                        return Apply2GValidation(vacProof);
                    }

                    // Booster validation: only DGCs certifying a completed vaccination cycle + a booster dose are valid
                    else if (validationMode == ValidationType.Booster)
                    {

                        return ApplyBoosterValidation(vacProof);

                    }
                }
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "An exception occurred during validation");
                throw e;
            }
            return null;
        }

        private SignedDGC ApplyBoosterValidation(SignedDGC vacProof)
        {
            /*
             * !!!IMPORTANT WARNING!!!
             * The code contained into the "if" from line 154 is not tested because there weren't enough test assets. Proceed at your own risk!
             * If you have testing assets, please do use them to test this code using the ValidationTests class
             */

            // if DGC === test, then INVALID
            if (vacProof.Dgc.Tests != null && vacProof.Dgc.Recoveries == null && vacProof.Dgc.Vaccinations == null)
            {
                vacProof.IsInvalid = true;
                return vacProof;
            }

            // if DGC == vaccination, there are additional parameters to check
            if (vacProof.Dgc.Vaccinations != null)
            {
                foreach (var v in vacProof.Dgc.Vaccinations)
                {
                    // we need to check the number of the administered dose (dn) to the required doses (sd) ratio
                    // if the ratio is positive (dn >= sd), the DGC could be VALID or TEST_NEEDED

                    if (v.DoseNumber >= v.SeriesOfDoses)
                    {
                        // we also need to check the medicinal product. If it is Johnson & Johnson, then the vaccination cycle is single dose
                        if (v.MedicinalProduct == "EU/1/20/1525") // Janssen vaccine
                        {
                            if ((v.DoseNumber == v.SeriesOfDoses) && v.DoseNumber < 2) 
                            {
                                // Since J&J vaccination cycle is completed with a single dose, then the cycle is completed but no booster dose was administered. TEST_NEEDED
                                vacProof.TestNeeded = true;
                                return vacProof;
                            }
                        }
                        else 
                        {
                            //if other medicinal products != J&J were used, then a vaccination cycle is completed with two administered doses

                            if ((v.DoseNumber == v.SeriesOfDoses) && v.DoseNumber < 3)
                            {
                                // vaccination cycle completed, but no booster dose administered. TEST_NEEDED
                                vacProof.TestNeeded = true;
                                return vacProof;
                            }
                        }
                        // otherwise, if ratio is over 1 (dn > sd), then the vaccination cycle is complete, and a booster dose was administered. VALID
                        return vacProof;
                    }
                    else
                    {
                        // dn < sd. Vaccination cycle not completed. INVALID
                        vacProof.IsInvalid = true;
                        return vacProof;
                    }
                }
            }

            // if DGC == recovery, then TEST_NEEDED
            if (vacProof.Dgc.Recoveries != null)
            {
                vacProof.TestNeeded = true;
                return vacProof;
            }

            //if we reach this code, there was an error.
            _logger.LogCritical($"ValidationService.ApplyBoosterValidation: unreachable code block reached. Setting DGC as invalid.");
            vacProof.IsInvalid = true;
            return vacProof;
        }

        private SignedDGC Apply2GValidation(SignedDGC vacProof)
        {
            // if dgc is invalid, return without checking other validation types
            if (vacProof.IsInvalid)
            {
                return vacProof;
            }

            // if DGC == test, then INVALID
            if (vacProof.Dgc.Tests != null && vacProof.Dgc.Recoveries == null && vacProof.Dgc.Vaccinations == null)
            {
                vacProof.IsInvalid = true;
                return vacProof;
            }

            // else (recovery or vaccination), test stays valid
            return vacProof;
        }

        async Task ApplyRules(SignedDGC vacProof)
        {
            await ApplyExpirationDate(vacProof);
        }
        async Task ApplyExpirationDate(SignedDGC vacProof)
        {
            var rules = await _cachingService.GetRules();
            var recovery = IsRecoveryInvalid(vacProof.Dgc);
            var vaccination = IsVaccinationInvalid(vacProof.Dgc, rules);
            var test = IsTestInvalid(vacProof.Dgc, rules);
            var blaklist = IsBlackListed(vacProof.Dgc, rules);
            if (recovery.GetValueOrDefault() || vaccination.GetValueOrDefault() || test.GetValueOrDefault() || blaklist.GetValueOrDefault())//default is false
                vacProof.IsInvalid = true;
            else
                vacProof.IsInvalid = GetActualDate() > vacProof.CertificateExpirationDate;
        }
        bool? IsTestInvalid(EU_DGC dgc, List<RemoteRule> rules)
        {
            try
            {
                var last = dgc.Tests?.OrderByDescending(x => x.SampleCollectionDate).FirstOrDefault();
                if (last == default)
                    return default;
                const string RapidType = "LP217198-3";
                const string MolecularType = "LP6464-4";
                string testTypeName = default;
                if (last.TestType == MolecularType)
                    testTypeName = "molecular";
                else if (last.TestType == RapidType)
                    testTypeName = "rapid";
                Func<RemoteRule, bool> predicate = x => x.Name.Contains("_test_");
                if (testTypeName != default)
                    predicate = x => x.Name.Contains(testTypeName) && x.Name.Contains("_test_");
                var applicableRules = rules.Where(predicate);
                int hoursStart = 0;
                int.TryParse(applicableRules.FirstOrDefault(x => x.Name.Contains("start"))?.Value, out hoursStart);
                if (GetActualDate() < last.SampleCollectionDate.AddHours(hoursStart)) //if it is too early, invalid
                    return true;

                int hoursEnd = 0;
                int.TryParse(applicableRules.FirstOrDefault(x => x.Name.Contains("end"))?.Value, out hoursEnd);
                if (GetActualDate() > last.SampleCollectionDate.AddHours(hoursEnd)) //if it is too late, invalid
                    return true;
                return false;
            }
            catch (Exception e)
            {
                return default;
            }
        }
        bool? IsVaccinationInvalid(EU_DGC dgc, List<RemoteRule> rules)
        {
            try
            {
                var last = dgc.Vaccinations?.OrderByDescending(x => x.Date).FirstOrDefault();
                if (last == default)
                    return default;
                var applicableRules = rules.Where(x => x.Type == last.MedicinalProduct);
                var isComplete = last.DoseNumber >= last.SeriesOfDoses;
                string ruleNameStart = $"vaccine_start_day_{(isComplete ? "complete" : "not_complete")}";
                string ruleNameEnd = $"vaccine_end_day_{(isComplete ? "complete" : "not_complete")}";

                int daysAfterStart = 0;
                int.TryParse(applicableRules.FirstOrDefault(x => x.Name == ruleNameStart)?.Value, out daysAfterStart);

                int daysFromEnd = 0;
                int.TryParse(applicableRules.FirstOrDefault(x => x.Name == ruleNameEnd)?.Value, out daysFromEnd);

                if (GetActualDate() < last.Date.AddDays(daysAfterStart)) //if it is too early, invalid
                    return true;
                if (GetActualDate() > last.Date.AddDays(daysFromEnd)) //if it is too late, invalid
                    return true;
                return false;//otherwise return false
            }
            catch (Exception e)
            {
                return default;
            }
        }

        bool? IsBlackListed(EU_DGC dgc, List<RemoteRule> rules)
        {
            try
            {
                var blacklistRule = rules.SingleOrDefault(x => x.Name == "black_list_uvci");

                if (!string.IsNullOrWhiteSpace(blacklistRule?.Value))
                {
                    foreach (var vac in dgc.Vaccinations)
                    {
                        if (blacklistRule.Value.IndexOf(vac.CertificateId, StringComparison.OrdinalIgnoreCase) >= 0) return true;
                    }
                }

                return false; //otherwise return false
            }
            catch (Exception e)
            {
                return default;
            }
        }

        bool? IsRecoveryInvalid(EU_DGC dgc)
        {
            try
            {
                var date = dgc.Recoveries?.OrderByDescending(x => x.ValidUntil).FirstOrDefault()?.ValidUntil.DateTime;
                if (date == default)
                    return default;
                return GetActualDate() > date;
            }
            catch (Exception e)
            {
                return default;
            }
        }
        DateTime GetActualDate()
        {
            return DateTime.Now;
        }
        protected static byte[] ZlibDecompression(byte[] compressedData)
        {
            if (compressedData[0] == 0x78)
            {
                var outputStream = new MemoryStream();
                using (var compressedStream = new MemoryStream(compressedData))
                using (var inputStream = new InflaterInputStream(compressedStream))
                {
                    inputStream.CopyTo(outputStream);
                    outputStream.Position = 0;
                    return outputStream.ToArray();
                }
            }
            else
            {
                // The data is not compressed
                return compressedData;
            }
        }

        protected static async Task<byte[]> VerifySignedData(byte[] signedData, SignedDGC vacProof, CertificateManager certificateManager, ILogger<ValidationService> logger)
        {
            DGCVerifier verifier = new DGCVerifier(certificateManager, logger);
            return await verifier.VerifyAsync(signedData, vacProof);
        }

        protected static byte[] Base45Decoding(byte[] encodedData)
        {
            byte[] uncodedData = Base45.Decode(encodedData);
            return uncodedData;
        }

        protected static EU_DGC GetVaccinationProofFromCbor(byte[] cborData)
        {
            CBORObject cbor = CBORObject.DecodeFromBytes(cborData, CBOREncodeOptions.Default);
            string json = cbor.ToJSONString();
            EU_DGC vacProof = EU_DGC.FromJson(cbor.ToJSONString());
            return vacProof;
        }


    }
}
