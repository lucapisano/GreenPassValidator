using System;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto;

namespace DGCValidator.Services.CWT.Certificates
{
    /**
     * A functional interface for finding certificates that may be used to verify a HCERT (see {@link HCertVerifier}).
     * 
     * @author Henrik Bengtsson (henrik@sondaica.se)
     * @author Martin Lindström (martin@idsec.se)
     * @author Henric Norlander (extern.henric.norlander@digg.se)
     */
    public interface ICertificateProvider
    {

        /**
         * Given a country code and a key identifier the method finds all certificates that matches this criteria.
         * <p>
         * At least one of the criteria is set in a call.
         * </p>
         * <p>
         * If the key identifier (kid) is {@code null} the provider should return all certificates for the given country.
         * </p>
         * <p>
         * If the country code is {@code null} the provider should return all certificates matching the key identifier.
         * </p>
         * 
         * @param country
         *          the two-letter country code
         * @param kid
         *          the key identifier
         * @return a list of certificates (never null)
         */
        List<AsymmetricKeyParameter> GetCertificates(String country, byte[] kid);

    }
}
