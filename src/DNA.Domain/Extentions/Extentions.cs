using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DNA.Domain.Extentions {
    public static class Extentions {

        public static bool IsNumeric(this Type type) {
            switch (Type.GetTypeCode(type)) {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static string ToTitleCase(this string str, string lng = "tr") {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            string result = Regex.Replace(str, @"((^[a-z]+)|([0-9]+)|([A-Z]{1}[a-z]+)|([A-Z]+(?=([A-Z][a-z])|($)|([0-9]))))", (m) => {
                return m.Value + " ";
            });
            return result.Trim().Replace("-", "").Replace("_", "").Replace("  ", " ").Trim();
        }

        public static string ToCamelCase(this string str, string lng = "tr") {
            Regex pattern = new Regex(@"[A-Z]{2,}(?=[A-Z][a-z]+[0-9]*|\b)|[A-Z]?[a-z]+[0-9]*|[A-Z]|[0-9]+");
            var strings = new List<string>();
            foreach (Match item in pattern.Matches(str))
                strings.Add(item.Value);

            var result = new string(
              new CultureInfo(lng, false)
                .TextInfo
                .ToTitleCase(string.Join(" ", strings).ToLower())
                .Replace(" ", "")
                .Select((x, i) => i == 0 ? char.ToLower(x) : x)
                .ToArray()
            );
            return result;
        }

        public static string ToProperCase(this string text) {
            //const string pattern = @"(?<=\w)(?=[A-Z])";
            const string pattern = @"(?<=[^A-Z])(?=[A-Z])";
            string result = Regex.Replace(text, pattern, " ", RegexOptions.None);
            return result.Substring(0, 1).ToUpper() + result.Substring(1);
        }
    }
}
