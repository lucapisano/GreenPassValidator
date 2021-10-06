using System;
using System.Collections.Generic;
using System.Text;

namespace GreenPass.Extensions
{
    public static class Base64Extension
    {
        public static string ToBase64Url(this byte[] data)
        {
            return Convert.ToBase64String(data)
                .Replace("+", "-")
                .Replace("/", "_");
        }
        public static byte[] FromBase64Url(this string input)
        {

            return Convert.FromBase64String(
                input.Replace("-", "+")
                .Replace("_", "/"));
        }
    }
}
