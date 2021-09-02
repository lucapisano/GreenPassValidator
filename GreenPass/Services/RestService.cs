using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DGCValidator.Services.CWT;
using DGCValidator.Services.CWT.Certificates;
using DGCValidator.Services.DGC.ValueSet;
using GreenPass.Options;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using RestSharp;

namespace DGCValidator.Services
{
    public class RestService : IRestService
    {
        private readonly IServiceProvider _sp;
        private readonly IOptions<ValidatorOptions> _opt;
        HttpClient client;

        public RestService(IOptions<ValidatorOptions> opt)
        {
            _opt = opt;
            client = new HttpClient();
        }

        public async Task<DSC_TL> RefreshTrustListAsync()
        {
            DSC_TL trustList = new DSC_TL();
            Uri uri = new Uri(_opt.Value.CertificateListProviderUrl);
            try
            {
                var res = new RestClient().Get(new RestRequest(uri));
                if (res.IsSuccessful)
                {
                    string content = res.Content;
                    // Verify signature
                    byte[] payload = Verify(content);
                    if( payload != null)
                    {
                        trustList = DSC_TL.FromJson(Encoding.UTF8.GetString(payload));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"error in retrieving Trust List from url {_opt.Value.CertificateListProviderUrl}", ex);
            }

            return trustList;
        }

        private byte[] Verify(string content)
        {
            try
            {
                string[] contents = content.Split(".");
                byte[] headerBytes = Base64UrlDecode(contents[0]);
                byte[] payloadBytes = Base64UrlDecode(contents[1]);
                byte[] signatureBytes = Base64UrlDecode(contents[2]);

                DSC_TL_HEADER header = DSC_TL_HEADER.FromJson(Encoding.UTF8.GetString(headerBytes));

                byte[] x5c = Convert.FromBase64String(header.X5C[0]);

                String x5cString = Encoding.UTF8.GetString(x5c);

                X509CertificateParser parser = new X509CertificateParser();
                X509Certificate cert = parser.ReadCertificate(x5c);

                AsymmetricKeyParameter jwsPublicKey = cert.GetPublicKey();

                ISigner signer = SignerUtilities.GetSigner("SHA256withECDSA");
                signer.Init(false, jwsPublicKey);

                /* Get the bytes to be signed from the string */
                var msgBytes = Encoding.ASCII.GetBytes(contents[0] + "." + contents[1]);

                /* Calculate the signature and see if it matches */
                signer.BlockUpdate(msgBytes, 0, msgBytes.Length);
                byte[] derSignature = ToDerSignature(signatureBytes);
                bool result = signer.VerifySignature(derSignature);
                if (result)
                {
                    return payloadBytes;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"error verifying Trust List signature. The list may be corrupted",ex );
            }
            return null;
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

        private byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 1: output += "==="; break; // Three pad chars
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default: throw new Exception("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }
    }
}
