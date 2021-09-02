using System;
using System.Collections;
using System.Collections.Generic;

namespace DGCValidator.Services.CWT
{
    /**
     * ASN.1 encoding support.
     *
     * @author Henrik Bengtsson (henrik@sondaica.se)
     * @author Martin Lindström (martin@idsec.se)
     * @author Henric Norlander (extern.henric.norlander@digg.se)
     */
    public class ASN1
    {
        private static byte[] SEQUENCE_TAG = new byte[] { 0x30 };

        /**
         * Converts the supplied bytes into the ASN.1 DER encoding for an unsigned integer.
         * 
         * @param i
         *          the byte array to convert
         * @return the DER encoding
         */
        public static byte[] ToUnsignedInteger(byte[] i)
        {
            int offset = 0;
            while (offset < i.Length && i[offset] == 0)
            {
                offset++;
            }
            if (offset == i.Length)
            {
                return new byte[] { 0x02, 0x01, 0x00 };
            }
            int pad = 0;
            if ((i[offset] & 0x80) != 0)
            {
                pad++;
            }

            int length = i.Length - offset;
            byte[] der = new byte[2 + length + pad];
            der[0] = 0x02;
            der[1] = (byte)(length + pad);
            Array.Copy(i, offset, der, 2 + pad, length);

            return der;
        }

        /**
         * Convert the supplied input to an ASN.1 Sequence.
         * 
         * @param seq
         *          the data in the sequence
         * @return the DER encoding
         */
        public static byte[] ToSequence(List<byte[]> seq)
        {
            byte[] seqBytes = ToBytes(seq);
            List<byte[]> seqList = new List<byte[]>
            {
                SEQUENCE_TAG
            };
            if (seqBytes.Length <= 127)
            {
                seqList.Add(new byte[] { (byte)seqBytes.Length });
            }
            else
            {
                seqList.Add(new byte[] { (byte)0x81, (byte)seqBytes.Length });
            }
            seqList.Add(seqBytes);

            return ToBytes(seqList);
        }

        private static byte[] ToBytes(List<byte[]> bytes)
        {
            int len = 0;
            foreach (byte[] r in bytes)
            {
                len += r.Length;
            }

            byte[] b = new byte[len];
            len = 0;
            foreach (byte[] r in bytes)
            {
                Array.Copy(r, 0, b, len, r.Length);
                len += r.Length;
            }

            return b;
        }
    }
}
