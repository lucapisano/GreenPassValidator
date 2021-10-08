using System;
using System.Collections.Generic;
using System.Text;

namespace GreenPass.Extensions
{
    public static class Converters
    {
        public static byte[] HexStringToByteArray(this string rawData)
        {
            if (string.IsNullOrEmpty(rawData))
            {
                throw new ArgumentNullException("rawData");
            }
            if (rawData.Length % 2 != 0)
            {
                throw new ArgumentException("Input must have an even length", "rawData");
            }

            byte[] result = new byte[rawData.Length / 2];
            for (int i = 0; i < result.Length; i++)
            {
                var byteInString = rawData.Substring(i * 2, 2);
                result[i] = Convert.ToByte(byteInString, 16);
            }
            return result;
        }
        public static string ByteArrayToHexString(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.Length == 0)
            {
                return string.Empty;
            }
            StringBuilder hex = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
