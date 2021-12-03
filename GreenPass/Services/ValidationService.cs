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

        public ValidationService(CertificateManager certificateManager, IServiceProvider sp)
        {
            _certManager = certificateManager;
            _cachingService = sp.GetRequiredService<CachingService>();
            _logger = sp.GetService<ILogger<ValidationService>>();
        }

        public async Task<SignedDGC> Validate(String codeData)
        {
            try
            {
                // The base45 encoded data shoudl begin with HC1
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
                    return vacProof;
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

        protected static async Task<byte[]> VerifySignedData(byte[] signedData, SignedDGC vacProof, CertificateManager certificateManager, ILogger logger)
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
