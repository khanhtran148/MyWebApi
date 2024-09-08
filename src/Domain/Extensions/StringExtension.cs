using System;
using System.Text.RegularExpressions;

namespace MyWebApi.Domain.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        ///  Control codes : https://en.wikipedia.org/wiki/C0_and_C1_control_codes#C0
        /// </summary>
        private static readonly Regex _controlCodes = new(@"[\u0000-\u0008\u000B-\u000C\u000E-\u000F\u0010-\u0019\u001A-\u001F\u007f]", RegexOptions.Compiled, TimeSpan.FromSeconds(5));

        public static string EscapeInvalidXmlChars(this string text)
        {
            return string.IsNullOrEmpty(text) ? "" : _controlCodes.Replace(text, "");
        }
    }
}
