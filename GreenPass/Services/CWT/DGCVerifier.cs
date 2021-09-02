using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DGCValidator.Services.CWT.Certificates;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;

/**
* A HCert verifier class.
*
* @author Henrik Bengtsson (henrik@sondaica.se)
* @author Martin Lindström (martin@idsec.se)
* @author Henric Norlander (extern.henric.norlander@digg.se)
*/
namespace DGCValidator.Services.CWT
{
    public class DGCVerifier
    {
        private readonly ICertificateProvider certificateProvider;

        public DGCVerifier(ICertificateProvider certificateProvider)
        {
            this.certificateProvider = certificateProvider;
        }

        /**
         * Verifies the supplied signed DGC. If verification is successful the method returns the contained HCERT
         * (eu_hcert_v1) in its binary representation.
         * 
         * @throws SignatureException
         *           if signature validation fails
         * @throws CertificateExpiredException
         *           if the DGC has expired
         */
        public async Task<byte[]> VerifyAsync(byte[] signedDGC, SignedDGC vacProof)
        {
            CoseSign1_Object obj = CoseSign1_Object.Decode(signedDGC);

            byte[] kid = obj.GetKeyIdentifier();
            string country = obj.GetCwt().GetIssuer();

            vacProof.IssuingCountry = country;

            if (kid == null && country == null)
            {
                throw new Exception("Signed DCC does not contain kid or country - cannot find certificate");
            }

            List<AsymmetricKeyParameter> certs = await certificateProvider.GetCertificates(country, kid);

            foreach (AsymmetricKeyParameter cert in certs)
            {
                Console.WriteLine("Attempting HCERT signature verification using certificate");// '{0}'", cert.Subject);//getSubjectX500Principal().getName()) ;

                try {
                    byte[] key = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(cert).GetEncoded();
                    obj.VerifySignature(key);
                    string keyString = System.Text.Encoding.UTF8.GetString(key);
                    Console.WriteLine("HCERT signature verification succeeded using certificate");// '{0}'", cert.Subject); //getSubjectX500Principal().getName());
                }
                catch (Exception e)
                {
                    Console.WriteLine(String.Format("HCERT signature verification failed using certificate '{0}' - {1}",cert, e.Message));
                    continue;
                }

                // OK, before we are done - let's ensure that the HCERT hasn't expired.
                CWT cwt = obj.GetCwt();

                DateTime? expiration = cwt.GetExpiration();
                if (expiration.HasValue)
                {
                    vacProof.ExpirationDate = expiration.Value;
                    if (DateTime.UtcNow.CompareTo(expiration) >= 0)
                    {
                        throw new CertificateExpiredException(string.Format("DCC has expired {0}",expiration.Value));
                    }
                }
                else
                {
                    Console.WriteLine("Signed HCERT did not contain an expiration time - assuming it is valid");
                }
                DateTime? issuedAt = cwt.GetIssuedAt();
                if(issuedAt.HasValue)
                {
                    vacProof.IssuedDate = issuedAt.Value;
                }
                // OK, everything looks fine - return the DGC
                return cwt.GetDgcV1();
            }

            if (certs.Count<=0)
            {
                throw new CertificateUnknownException("No signer certificates could be found");
            }
            else
            {
                throw new CertificateValidationException("Signature verification failed for all attempted keys");
            }
        }
    }
}
