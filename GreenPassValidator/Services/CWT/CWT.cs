using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PeterO.Cbor;

namespace DGCValidator.Services.CWT
{

    /**
     * A representation of a CWT according to <a href="https://tools.ietf.org/html/rfc8392">RFC 8392</a>.
     * 
     * @author Henrik Bengtsson (henrik@sondaica.se)
     * @author Martin Lindström (martin@idsec.se)
     * @author Henric Norlander (extern.henric.norlander@digg.se)
     */
    public class CWT
    {
        /** HCERT message tag. */
        public static int HCERT_CLAIM_KEY = -260;

        /** The message tag for eu_hcert_v1 that is added under the HCERT claim. */
        public static int EU_HCERT_V1_MESSAGE_TAG = 1;

        /** The CBOR CWT message tag. */
        public static int MESSAGE_TAG = 61;

        /** For handling of DateTime. */
        private static CBORDateTimeConverter dateTimeConverter = new CBORDateTimeConverter();

        /** The CBOR representation of the CWT. */
        private CBORObject CwtObject;

        /**
            * Constructor creating an empty CWT.
            */
        public CWT() {
            CwtObject = CBORObject.NewMap();
        }

        /**
            * Constructor creating a CWT from a supplied encoding.
            * 
            * @param data
            *          the encoding
            * @throws CBORException
            *           if the supplied encoding is not a valid CWT
            */
        public CWT(byte[] data){
            CBORObject obj = CBORObject.DecodeFromBytes(data);
            if (obj.Type != CBORType.Map) {
                throw new CBORException("Not a valid CWT");
            }
            CwtObject = obj;
        }

        /**
            * Decodes the supplied data into a Cwt object.
            *
            * @param data
            *          the encoded data
            * @return a Cwt object
            * @throws CBORException
            *           if the supplied encoding is not a valid CWT
            */
        public static CWT Decode(byte[] data){
            return new CWT(data);
        }

        /**
            * Gets the binary representation of the CWT.
            * 
            * @return a byte array
            */
        public byte[] Encode() {
            return CwtObject.EncodeToBytes();
        }

        /**
            * Gets the "iss" (issuer) claim.
            * 
            * @return the issuer value, or null
            */
        public String GetIssuer() {
            CBORObject cbor = CwtObject[1];
            if (cbor == null)
            {
                return null;
            }
            return cbor.AsString();
        }

        /**
         * Gets the "sub" (subject) claim.
         * 
         * @return the subject value, or null
         */
        public String GetSubject() {
            CBORObject cbor = CwtObject[2];
            if (cbor == null)
            {
                return null;
            }
            return cbor.AsString();
        }

        /**
         * Gets the values for the "aud" claim
         * 
         * @return the value, or null
         */
        public List<String> getAudience()
        {
            CBORObject aud = CwtObject[3];
            if (aud == null)
            {
                return null;
            }
            if (aud.Type == CBORType.Array)
            {
                ICollection<CBORObject> values = aud.Values;
                List<string> audList = new List<string>();
                foreach (CBORObject o in values){
                    audList.Add(o.AsString());
                }
                return audList;
            }
            else
            {
                return new List<string> { aud.AsString() };
            }
        }

        /**
        * Gets the value of the "exp" (expiration time) claim.
        * 
        * @return the instant, or null
        */
        public DateTime? GetExpiration() {
            return dateTimeConverter.FromCBORObject(CwtObject[4]);
        }

        /**
         * Gets the value of the "nbf" (not before) claim.
         * 
         * @return the instant, or null
         */
        public DateTime? GetNotBefore() {
            return dateTimeConverter.FromCBORObject(CwtObject[5]);
        }

  
        /**
         * Gets the value of the "iat" (issued at) claim.
         * 
         * @return the instant, or null
         */
        public DateTime? GetIssuedAt() {
            return dateTimeConverter.FromCBORObject(CwtObject[6]);
        }

 
        /**
         * Gets the value of the "cti" (CWT ID) claim.
         * 
         * @return the ID, or null
         */
        public byte[] GetCwtId() {
            CBORObject cbor = CwtObject[7];
            if (cbor == null)
            {
                return null;
            }
            return cbor.GetByteString();
        }

        /**
         * Gets the binary representation of a EU HCERT v1 structure.
         * 
         * @return the binary representation of a EU HCERT or null
         */
        public byte[] GetDgcV1()
        {
            CBORObject hcert = CwtObject[HCERT_CLAIM_KEY];
            if (hcert == null)
            {
                return null;
            }
            return hcert[EU_HCERT_V1_MESSAGE_TAG].EncodeToBytes();
        }


        /**
         * Gets the claim identified by {@code claimKey}.
         * 
         * @param claimKey
         *          the claim key
         * @return the claim value (in its CBOR binary encoding), or null
         */
        public CBORObject getClaim(int claimKey)
        {
            return CwtObject[claimKey];
        }

        /**
         * Gets the claim identified by {@code claimKey}.
         * 
         * @param claimKey
         *          the claim key
         * @return the claim value (in its CBOR binary encoding), or null
         */
        public CBORObject getClaim(string claimKey)
        {
            return CwtObject[claimKey];
        }

        /** {@inheritDoc} */
        public override string ToString() {
            return CwtObject.ToString();
        }
    }
}
