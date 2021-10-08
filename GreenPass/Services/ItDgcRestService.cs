using DGCValidator.Services.CWT;
using DGCValidator.Services.CWT.Certificates;
using GreenPass.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DGCValidator.Services
{
    public class ItDgcRestService : IRestService
    {
        private readonly IServiceProvider _sp;
        private readonly IOptions<ValidatorOptions> _opt;
        private RestClient client;

        public ItDgcRestService(IOptions<ValidatorOptions> opt)
        {
            _opt = opt;

            //initiate a rest client
            client = new RestClient();
        }

        public async Task<DSC_TL> RefreshTrustListAsync()
        {
            var trustList = new DSC_TL();

            // get itDGC api base url
            var providerUrl = _opt.Value.CertificateListProviderUrl;
            Uri baseUri;
            if (!Uri.TryCreate(providerUrl, UriKind.Absolute, out baseUri))
            {
                throw new Exception($"The URI provided in the configuation is not valid. Base url: {providerUrl}");
            }
            var statusPath = new Uri(baseUri, "signercertificate/status");
            var updatePath = new Uri(baseUri, "signercertificate/update");

            // get status
            var validKids = new string[0];
            try
            {
                // get
                var statusResponse = client.Get(new RestRequest(statusPath));

                // parse to JSON and get the array
                validKids = JArray.Parse(statusResponse.Content).ToObject<string[]>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting status: {ex.Message}", ex);
            }

            // prepare for the loop
            string targetKid = ""; //the KID of the target Signer Certificate.
            string signerCert = ""; //The signer's certificate. Need to append begin and end certificate.
            List<string> signerCertificates = new List<string>();
            string resumeToken = "0";
            trustList.DscTrustList = new Dictionary<string, DscTrust>();
            //TODO: loop through updates. 
            var getNextUpdate = false;
            var RESUME_TOKEN_HEADER_KEY = "X-RESUME-TOKEN";
            var KID = "X-KID";
            do
            {
                RestRequest request = null;
                //get update
                try
                {
                    request = new RestRequest(updatePath);
                    request.AddHeader(RESUME_TOKEN_HEADER_KEY, resumeToken);
                }
                catch (Exception e)
                {
                    throw new Exception($"Error in retrieving certificate from {updatePath}", e);
                }

                IRestResponse response = null;
                try
                {
                    response = client.Execute(request, Method.GET);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while getting the HTTP response. ", ex);
                }
                var responseHeaders = response.Headers.ToList();
                var kid = responseHeaders.Find(x => x.Name == KID);
                //get kid & resume token from headers
                if (kid == null) break;
                else targetKid = kid.Value.ToString();

                //check if received kid is valid
                if (validKids.Contains(targetKid))
                {
                    var responseText = string.Empty;
                    try
                    {
                        responseText = response.Content;
                        resumeToken = responseHeaders.Find(x => x.Name == RESUME_TOKEN_HEADER_KEY)?.Value.ToString();
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Error while reading reponse data: {e.Message}", e);
                    }
                    signerCert = responseText;


                    // prepare JWK
                    var bytes = Encoding.ASCII.GetBytes(signerCert);
                    var jwk = Key.LoadFromX509(bytes) ;

                    //  prepare trustList
                    var certificate = new X509Certificate2(bytes);
                    // find certificate issuer
                    var iss = certificate.Issuer.Split(',');
                    var cc = string.Empty;
                    for (int i = 0; i < iss.Length; i++)
                    {
                        if (iss[i].Contains("C="))
                        {
                            cc = iss[i].Substring(iss[i].IndexOf("=")+1);
                        }
                    }

                    // now we have a certificate, ready to be pushed in the country DSC list                    

                    // check if the country DSC list is already present in the trustlist. 
                    if (!trustList.DscTrustList.ContainsKey(cc))
                    {
                        // if not, create a new one.
                        trustList.DscTrustList.Add(cc, new DscTrust());
                    }

                    // get the country DSC list (maybe it's empty, since could be just created)
                    var countryDscList = new DscTrust();
                    if (trustList.DscTrustList.TryGetValue(cc, out countryDscList))
                    {
                        // prepare a list of key, preserving existing ones
                        var keys = new List<Key>();
                        if (countryDscList.Keys != null)
                        {
                            keys.AddRange(countryDscList.Keys);
                        }

                        // append the new key
                        keys.Add(jwk);
                        countryDscList.Keys = keys.ToArray();
                    }
                    else
                    {
                        // cannot get the country DSC list! argh, something went wrong
                        throw new Exception($"Cannot find nor create DSC trust list for sountry {cc}");
                    }

                }
                // contnue if I got a 200 OK && a new resume token.
                getNextUpdate = (response.StatusCode == HttpStatusCode.OK) && (!string.IsNullOrEmpty(resumeToken));
            } while (getNextUpdate);


            // set issuer
            trustList.Iss = baseUri.ToString();

            // set issue date
            var now = DateTimeOffset.Now;
            trustList.Iat = now.ToUnixTimeSeconds();

            // set expiration to tomorrow at 3 AM
            // WARN this is an arbitrary rule
            // TODO find a better rule
            trustList.Exp = now.AddHours(24-now.Hour+3).ToUnixTimeSeconds();
            return trustList;
        }
        

        private static byte[] ToDerSignature(byte[] jwsSig)
        {
            int len = jwsSig.Length / 2;
            byte[] r = new byte[len];
            byte[] s = new byte[len];
            Array.Copy(jwsSig, r, len);
            Array.Copy(jwsSig, len, s, 0, len);

            List<byte[]> seq = new List<byte[]>();
            seq.Add(ASN1.ToUnsignedInteger(r));
            seq.Add(ASN1.ToUnsignedInteger(s));

            byte[] derSeq = ASN1.ToSequence(seq);
            return derSeq;
        }
    }
}
