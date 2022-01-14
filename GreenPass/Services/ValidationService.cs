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
        private readonly CertificateManager _certManager;
        private CachingService _cachingService;
        private ILogger<ValidationService> _logger;

        public enum ValidationType
        {
            ThreeG, // 3G (vaccine, recovery, test): base validation
            TwoG,   // 2G (vaccine, recovery): reinforced validation
            Booster // booster validation
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
                    await ApplyRules(vacProof);

                    // if dgc is invalid, return without checking other validation types
                    if (vacProof.IsInvalid)
                    {
                        return vacProof;
                    }

                    else
                    {
                        // check for different validation types

                        // if validationMode == ThreeG (3G), it is a base validation
                        if (validationMode == ValidationType.ThreeG)
                        {
                            return vacProof;
                        }

                        // TwoG (2G): "reinforced" validation, tests become invalid while vaccinations & recoveries stay valid
                        else if (validationMode == ValidationType.TwoG)
                        {
                            // if DGC == test, then INVALID
                            if (vacProof.Dgc.Tests != null && vacProof.Dgc.Recoveries == null && vacProof.Dgc.Vaccinations == null)
                            {
                                vacProof.IsInvalid = true;
                                return vacProof;
                            }

                            // else (recovery or vaccination), test stays valid
                            return vacProof;
                        }

                        // Booster validation
                        else if (validationMode == ValidationType.Booster)
                        {
                            // if DGC == test, then INVALID
                            if (vacProof.Dgc.Tests != null && vacProof.Dgc.Recoveries == null && vacProof.Dgc.Vaccinations == null)
                            {
                                vacProof.IsInvalid = true;
                                return vacProof;
                            }

                            //else if recovery, then the DGC is valid, but it still needs to show a valid Covid-19 test so result is NEED_TESTING
                            else if (vacProof.Dgc.Tests == null && vacProof.Dgc.Recoveries != null && vacProof.Dgc.Vaccinations == null)
                            {
                                vacProof.TestNeeded = true;
                                return vacProof;
                            }

                            // !!! IMPORTANT WARNING: the code contained into the "if" from line 148 is not tested because there weren't enough test assets. Proceed at your own risk!
                            // !!! If you have testing assets, please do use them to test this code

                            // else if vaccination, there are additional parameters required
                            else if (vacProof.Dgc.Tests == null && vacProof.Dgc.Recoveries == null && vacProof.Dgc.Vaccinations != null)
                            {
                                // we need to check the dose number (dn) to series of doses (sd) ratio
                                var vaccines = vacProof.Dgc.Vaccinations;
                                foreach (var v in vaccines)
                                {
                                    var ratio = v.DoseNumber / v.SeriesOfDoses;

                                    // if ratio is below 1, the vaccination cycle is incomplete. INVALID
                                    if (ratio < 1)
                                    {
                                        vacProof.IsInvalid = true;
                                        return vacProof;
                                    }

                                    // if ratio is over 1, then the vaccination cycle is completed and a booster dose was made. VALID
                                    else if (ratio > 1)
                                    {
                                        return vacProof;
                                    }

                                    //if ratio is = 1, then we have to consider different cases
                                    else if (ratio == 1)
                                    {
                                        // we need additional data: the medicinal product (mp), since the Johnson&Johnson vaccine cycle is mono-dose (so ratio could be = 1)
                                        if (v.MedicinalProduct == "EU/1/20/1525") // J&J vaccine
                                        {
                                            if (v.DoseNumber == 1 && v.SeriesOfDoses == 1)
                                            {
                                                // cycle completed, but no booster dose made. TEST NEEDED
                                                vacProof.TestNeeded = true;
                                                return vacProof;
                                            }
                                            else if (v.DoseNumber == 2 && v.SeriesOfDoses == 2)
                                            {
                                                // cycle completed, and booster dose made. VALID
                                                return vacProof;
                                            }
                                        }
                                        else //for every other medicinal product
                                        {
                                            if (v.DoseNumber == 1)
                                            {
                                                // vaccination cycle incomplete. INVALID
                                                vacProof.IsInvalid = true;
                                                return vacProof;
                                            }

                                            else if (v.DoseNumber == 2)
                                            {
                                                // vaccination cycle completed, no booster dose. TEST NEEDED
                                                vacProof.TestNeeded = true;
                                                return vacProof;
                                            }

                                            else if (v.DoseNumber == 3)
                                            {
                                                // vaccination cycle completed + booster dose. VALID
                                                return vacProof;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        
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
