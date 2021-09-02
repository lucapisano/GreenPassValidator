using System;
using System.Text;
/*
* MIT License
*
* Copyright 2021 Myndigheten för digital förvaltning (DIGG)
*/
namespace DGCValidator.Services
{
    /**
     * Implementation of Base45 encoding/decoding according to <a href=
     * "https://datatracker.ietf.org/doc/draft-faltstrom-base45/">https://datatracker.ietf.org/doc/draft-faltstrom-base45/</a>.
     *
     * @author Martin Lindström (martin@idsec.se)
     * @author Henrik Bengtsson (extern.henrik.bengtsson@digg.se)
     * @author Henric Norlander (extern.henric.norlander@digg.se)
     */
    public class Base45
    {
        private static readonly char[] Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:".ToCharArray();
        private static readonly int[] DecodingTable = new int[256];

        static Base45()
        {
            for (int i = 0; i < Base45.DecodingTable.Length; i++)
                DecodingTable[i] = -1;
            for (int i = 0; i < 45; i++)
                DecodingTable[Alphabet[i]] = i;
        }


        /**
         * Decodes the supplied input (which is the byte array representation of a Base45 string).
         *
         * @param src
         *          the Base45 string to decode
         * @return an allocated byte array
         */
        public static byte[] Decode(byte[] coded)
        {
            int uncodedLength = ((coded.Length / 3) * 2) + ((coded.Length % 3) / 2);
            byte[] output = new byte[uncodedLength];
            int ip = 0;
            int op = 0;
            while (ip < coded.Length)
            {
                int i0 = coded[ip++];
                int i1 = coded[ip++];
                int i2 = (ip < coded.Length ? coded[ip] : 0);
                if (i0 > 127 || i1 > 127 || i2 > 127)
                    throw new ArgumentException("Illegal character in Base45 encoded data.");
                int b0 = DecodingTable[i0];
                int b1 = (ip <= coded.Length ? DecodingTable[i1] : 0);
                int b2 = (ip < coded.Length ? DecodingTable[i2] : 0);
                if (b0 < 0 || b1 < 0 || b2 < 0)
                    throw new ArgumentException("Illegal character in Base45 encoded data.");
                int value = b0 + 45 * b1 + 45 * 45 * b2;
                int o0 = (value / 256);
                int o1 = (value % 256);
                output[op++] = (op < uncodedLength ? (byte)o0 : (byte)o1);
                if (op < uncodedLength)
                {
                    output[op++] = (byte)o1;
                }
                ip++;
            }
            return output;

        }

        /**
         * Decodes the supplied Base45 string.
         *
         * @param src
         *          the Base45 string to decode
         * @return an allocated byte array
         */
        public byte[] Decode(string src)
        {
            return Decode(Encoding.ASCII.GetBytes(src));
        }

    }
}

