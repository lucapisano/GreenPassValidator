using System;
using System.ComponentModel;
using PeterO.Cbor;

/*
 * MIT License
 * 
 * Copyright 2021 Myndigheten för digital förvaltning (DIGG)
 */
namespace DGCValidator.Services.CWT
{
    /**
     * Representation of the supported signature algorithms.
     * 
     * @author Henrik Bengtsson (extern.henrik.bengtsson@digg.se)
     * @author Martin Lindström (martin@idsec.se)
     * @author Henric Norlander (extern.henric.norlander@digg.se)
     */
    public class SignatureAlgorithm
    {
        /** ECDSA with SHA-256. */
        public static readonly CBORObject ES256 = CBORObject.FromObject(-7);

        /** ECDSA with SHA-384. */
        public static readonly CBORObject ES384 = CBORObject.FromObject(-35);

        /** ECDSA with SHA-512. */
        public static readonly CBORObject ES512 = CBORObject.FromObject(-36);

        /** RSASSA-PSS with SHA-256. */
        public static readonly CBORObject PS256 = CBORObject.FromObject(-37);

        /** RSASSA-PSS with SHA-384. */
        public static readonly CBORObject PS384 = CBORObject.FromObject(-38);

        /** RSASSA-PSS with SHA-512. */
        public static readonly CBORObject PS512 = CBORObject.FromObject(-39);


        public static String GetAlgorithmName(CBORObject cborValue)
        {
            switch (cborValue.AsInt32())
            {
                case -7:
                    return "SHA256withECDSA";
                case -35:
                    return "SHA384withECDSA";
                case -36:
                    return "SHA512withECDSA";
                case -37:
                    return "SHA256withRSA/PSS";
                case -38:
                    return "SHA384withRSA/PSS";
                case -39:
                    return "SHA512withRSA/PSS";
                default:
                    break;
            }
            return null;
        }
    }
}
