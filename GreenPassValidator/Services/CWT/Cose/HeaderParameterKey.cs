using System;
using PeterO.Cbor;

/*
 * MIT License
 * 
 * Copyright 2021 Myndigheten för digital förvaltning (DIGG)
 */
namespace DGCValidator.Services.CWT
{
    /**
     * Representation of COSE header parameter keys.
     * <p>
     * Only those relevant for our use case is represented.
     * </p>
     * 
     * @author Henrik Bengtsson (extern.henrik.bengtsson@digg.se)
     * @author Martin Lindström (martin@idsec.se)
     * @author Henric Norlander (extern.henric.norlander@digg.se)
     */
    public class HeaderParameterKey
    {

        /** Algorithm used for security processing. */
        public static readonly CBORObject ALG = CBORObject.FromObject(1);

        /** Critical headers to be understood. */
        public static readonly CBORObject CRIT = CBORObject.FromObject(2);

        /** This parameter is used to indicate the content type of the data in the payload or ciphertext fields. */
        public static readonly CBORObject CONTENT_TYPE = CBORObject.FromObject(3);

        /** This parameter identifies one piece of data that can be used as input to find the needed cryptographic key. */
        public static readonly CBORObject KID = CBORObject.FromObject(4);
    }
}
