using System;
using PeterO.Cbor;
using System.Collections.Generic;

using Org.BouncyCastle.Security;
using System.Security.Cryptography;

/*
 * MIT License
 * 
 * Copyright 2021 Myndigheten för digital förvaltning (DIGG)
 */
namespace DGCValidator.Services.CWT
{
    /**
     * A representation of a COSE_Sign1 object.
     *
     * @author Henrik Bengtsson (extern.henrik.bengtsson@digg.se)
     * @author Martin Lindström (martin@idsec.se)
     * @author Henric Norlander (extern.henric.norlander@digg.se)
     */
    public class CoseSign1_Object
    {
        /** The COSE_Sign1 message tag. */
        public static int MessageTag = 18;

        /** Should the message tag be included? The default is {@code true}. */
        private bool IncludeMessageTag = true;

        /** The protected attributes. */
        private CBORObject ProtectedAttributes;

        /** The encoding of the protected attributes. */
        private byte[] ProtectedAttributesEncoding;

        /** The unprotected attributes. */
        private CBORObject UnprotectedAttributes;

        /** The data content (data that is signed). */
        private byte[] Content;

        /** The signature. */
        private byte[] Signature;

        /** We don't support external data - so it's static. */
        private static byte[] ExternalData = new byte[0];

        /** The COSE_Sign1 context string. */
        private static String ContextString = "Signature1";

        /**
         * Default constructor.
         */
        public CoseSign1_Object()
        {
            ProtectedAttributes = CBORObject.NewMap();
            UnprotectedAttributes = CBORObject.NewMap();
        }

        /**
         * Constructor that accepts the binary representation of a signed COSE_Sign1 object.
         * 
         * @param data
         *          the binary representation of the COSE_Sign1 object
         * @throws CBORException
         *           for invalid data
         */
        public CoseSign1_Object(byte[] data)
        {
            CBORObject message = CBORObject.DecodeFromBytes(data);
            if (message.Type != CBORType.Array)
            {
                throw new CBORException("Supplied message is not a valid COSE security object");
            }

            // If the message is tagged, it must have the message tag for a COSE_Sign1 message.
            // We also handle the case where there is an outer CWT tag.
            //
            if (message.IsTagged) {
                if (message.GetAllTags().Length > 2) {
                    throw new CBORException("Invalid object - too many tags");
                }
                if (message.GetAllTags().Length == 2)
                {
                    if (CWT.MESSAGE_TAG != message.MostOuterTag.ToInt32Unchecked())
                    {
                        throw new CBORException(string.Format(
                          "Invalid COSE_Sign1 structure - Expected {0} tag - but was {1}",
                          CWT.MESSAGE_TAG, message.MostInnerTag.ToInt32Unchecked()));
                    }
                }
                if (MessageTag != message.MostInnerTag.ToInt32Unchecked())
                {
                    throw new CBORException(String.Format(
                      "Invalid COSE_Sign1 structure - Expected {0} tag - but was {1}",
                      MessageTag, message.MostInnerTag.ToInt32Unchecked()));
                }
            }

            if (message.Count != 4)
            {
                throw new CBORException(String.Format(
                  "Invalid COSE_Sign1 structure - Expected an array of 4 items - but array has {0} items", message.Count));
            }
            if (message[0].Type == CBORType.ByteString)
            {
                ProtectedAttributesEncoding = message[0].GetByteString();

                if (message[0].GetByteString().Length == 0)
                {
                    ProtectedAttributes = CBORObject.NewMap();
                }
                else
                {
                    ProtectedAttributes = CBORObject.DecodeFromBytes(ProtectedAttributesEncoding);
                    if (this.ProtectedAttributes.Count == 0)
                    {
                        this.ProtectedAttributesEncoding = new byte[0];
                    }
                }
            }
            else
            {
                throw new CBORException(String.Format("Invalid COSE_Sign1 structure - " +
                    "Expected item at position 1/4 to be a bstr which is the encoding of the protected attributes, but was {0}",
                  message[0].Type));
            }

            if (message[1].Type == CBORType.Map)
            {
                UnprotectedAttributes = message[1];
            }
            else
            {
                throw new CBORException(String.Format(
                  "Invalid COSE_Sign1 structure - Expected item at position 2/4 to be a Map for unprotected attributes, but was {0}",
                  message[1].Type));
            }

            if (message[2].Type == CBORType.ByteString)
            {
                Content = message[2].GetByteString();
            }
            else if (!message[2].IsNull)
            {
                throw new CBORException(String.Format(
                  "Invalid COSE_Sign1 structure - Expected item at position 3/4 to be a bstr holding the payload, but was {0}",
                  message[2].Type));
            }

            if (message[3].Type == CBORType.ByteString)
            {
                Signature = message[3].GetByteString();
            }
            else
            {
                throw new CBORException(String.Format(
                  "Invalid COSE_Sign1 structure - Expected item at position 4/4 to be a bstr holding the signature, but was {0}",
                  message[3].Type));
            }
        }

        /**
         * Decodes the supplied data into a CoseSign1_Object object.
         * 
         * @param data
         *          the encoded data
         * @return a CoseSign1_Object object
         * @throws CBORException
         *           if the supplied encoding is not a valid CoseSign1_Object
         */
        public static CoseSign1_Object Decode(byte[] data)
        {
            return new CoseSign1_Object(data);
        }

        /**
         * A utility method that looks for the key identifier (kid) in the protected (and unprotected) attributes.
         * 
         * @return the key identifier as a byte string
         */
        public byte[] GetKeyIdentifier()
        {
            CBORObject kid = ProtectedAttributes[HeaderParameterKey.KID];
            if( kid == null)
            {
                kid = UnprotectedAttributes[HeaderParameterKey.KID];
            }

            if (kid == null)
            {
                return null;
            }
            return kid.GetByteString();
        }

        /**
         * A utility method that gets the contents as a {@link Cwt}.
         * 
         * @return the CWT or null if no contents is available
         * @throws CBORException
         *           if the contents do not hold a valid CWT
         */
        public CWT GetCwt()
        {
            if (Content == null) {
                return null;
            }
            return CWT.Decode(Content);
        }

        /**
         * Verifies the signature of the COSE_Sign1 object.
         * <p>
         * Note: This method only verifies the signature. Not the payload.
         * </p>
         * 
         * @param publicKey
         *          the key to use when verifying the signature
         * @throws SignatureException
         *           for signature verification errors
         */
        public void VerifySignature(byte[] publicKey)
        {
            if (Signature == null) {
                throw new Exception("Object is not signed");
            }

            CBORObject obj = CBORObject.NewArray();
            obj.Add(ContextString);
            obj.Add(ProtectedAttributesEncoding);
            obj.Add(ExternalData);
            if (Content != null) {
                obj.Add(Content);
            }
            else {
                obj.Add(null);
            }

            byte[] signedData = obj.EncodeToBytes();

            // First find out which algorithm to use by searching for the algorithm ID in the protected attributes.
            //
            CBORObject registeredAlgorithm = ProtectedAttributes[HeaderParameterKey.ALG];
            if (registeredAlgorithm == null)
            {
                throw new Exception("No algorithm ID stored in protected attributes - cannot sign");
            }
            
            byte[] signatureToVerify = Signature;

            // For ECDSA, convert the signature according to section 8.1 of RFC8152.
            //
            if (registeredAlgorithm == SignatureAlgorithm.ES256
                || registeredAlgorithm == SignatureAlgorithm.ES384
                || registeredAlgorithm == SignatureAlgorithm.ES512)
            {

                signatureToVerify = ConvertToDer(Signature);
            }


            // Verify using the public key
            var pubkey = PublicKeyFactory.CreateKey(publicKey);

            var verifier = SignerUtilities.GetSigner(SignatureAlgorithm.GetAlgorithmName(registeredAlgorithm));
            verifier.Init(false, pubkey);
            verifier.BlockUpdate(signedData, 0, signedData.Length);
            var result = verifier.VerifySignature(signatureToVerify);

            if (!result)
            {
                throw new CertificateValidationException("Signature did not verify correctly");
            }
        }

        /**
         * Given a signature according to section 8.1 in RFC8152 its corresponding DER encoding is returned.
         * 
         * @param rsConcat
         *          the ECDSA signature
         * @return DER-encoded signature
         */
        private static byte[] ConvertToDer(byte[] rsConcat)
        {
            int len = rsConcat.Length / 2;
            byte[] r = new byte[len];
            byte[] s = new byte[len];
            Array.Copy(rsConcat, r, len);
            Array.Copy(rsConcat, len, s, 0, len);

            List<byte[]> seq = new List<byte[]>();
            seq.Add(ASN1.ToUnsignedInteger(r));
            seq.Add(ASN1.ToUnsignedInteger(s));

            return ASN1.ToSequence(seq);
        }
    }
}
