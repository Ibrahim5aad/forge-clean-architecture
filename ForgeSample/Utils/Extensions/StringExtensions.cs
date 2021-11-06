using System;

namespace ForgeSample.Utils.Extensions
{
    public static class StringExtensions
    {

        /// <summary>
        /// Base64 enconde a string
        /// </summary>
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string encodedText)
        {
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedText));
        }

    }
}
