using GreenPass.Extensions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DGCValidator.Services.CWT.Certificates
{
    public partial class Key
    {
        /// <summary>
        /// creates a Json Web Key from a byte array representing a X509 Certificate.
        /// </summary>
        /// <param name="rawData">The byte array representation of a X509 certificate.</param>
        /// <returns></returns>
        public static Key LoadFromX509(byte[] rawData)
        {
            if (rawData.Length <= 0) return null;
            var certificate = new X509Certificate2(rawData, string.Empty, X509KeyStorageFlags.Exportable);
            var ECDsaPk = certificate.GetECDsaPublicKey();
            var RsaPK = certificate.GetRSAPublicKey();
            var Jwk = new JsonWebKey();
            if (ECDsaPk == null && RsaPK == null) return null;

            if (ECDsaPk != null && RsaPK == null)
            {
                ECDsaSecurityKey pKey = new ECDsaSecurityKey(ECDsaPk);
                Jwk = JsonWebKeyConverter.ConvertFromECDsaSecurityKey(pKey);
            }
            else if (RsaPK != null && ECDsaPk == null)
            {
                RsaSecurityKey rpKey = new RsaSecurityKey(RsaPK);
                Jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(rpKey);
            }
            var certData = certificate.GetRawCertData();
            Key result = new Key()
            {
                Crv = Jwk.Crv,
                Kid = KidFromCertData(certData),
                Kty = Jwk.Kty,
                X5A = null,
                X5TS256 = CertDataToX5TS256(certData),
                Y = Jwk.Y,
                X = Jwk.X,
                N = Jwk.N,
                E = Jwk.E

            };

            return result;
        }

        private static string CertDataToX5TS256(byte[] data)
        {
           var sha256 = SHA256.Create();
            return sha256.ComputeHash(data)
                .ToBase64Url()
                .Replace("=", string.Empty);
        }
        private static string KidFromCertData(byte[] data)
        {
            var sha256 = SHA256.Create();
            return sha256.ComputeHash(data).Take(8).ToArray().ToBase64Url();
        }

    }
}

