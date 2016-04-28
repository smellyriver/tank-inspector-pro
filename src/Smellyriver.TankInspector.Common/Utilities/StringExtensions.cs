using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace Smellyriver.TankInspector.Common.Utilities
{

    public static class StringExtensions
    {
        public static string ToLiteral(this string input)
        {
            var literal = new StringBuilder(input.Length + 2);
            literal.Append("\"");
            foreach (var c in input)
            {
                switch (c)
                {
                    case '\'': literal.Append(@"\'"); break;
                    case '\"': literal.Append("\\\""); break;
                    case '\\': literal.Append(@"\\"); break;
                    case '\0': literal.Append(@"\0"); break;
                    case '\a': literal.Append(@"\a"); break;
                    case '\b': literal.Append(@"\b"); break;
                    case '\f': literal.Append(@"\f"); break;
                    case '\n': literal.Append(@"\n"); break;
                    case '\r': literal.Append(@"\r"); break;
                    case '\t': literal.Append(@"\t"); break;
                    case '\v': literal.Append(@"\v"); break;
                    default:
                        if (Char.GetUnicodeCategory(c) != UnicodeCategory.Control)
                        {
                            literal.Append(c);
                        }
                        else
                        {
                            literal.Append(((ushort)c).ToString("x4"));
                        }
                        break;
                }
            }
            literal.Append("\"");
            return literal.ToString();
        }

        public static object ConvertTo(this string value, Type targetType, TypeConverter converter = null, ITypeDescriptorContext context = null, CultureInfo culture = null)
        {
            object result;
            if (targetType == typeof(object) && converter == null)
                return value;
            else if (value.TryConvertTo(targetType, out result, converter, context, culture))
                return result;
            else
                throw new NotSupportedException("the specified value cannot be converted from string");
        }

        public static T ConvertTo<T>(this string value, TypeConverter converter = null, ITypeDescriptorContext context = null, CultureInfo culture = null)
        {
            return (T)value.ConvertTo(typeof(T), converter, context, culture);
        }

        public static bool TryConvertTo(this string value, Type targetType, out object result, TypeConverter converter = null, ITypeDescriptorContext context = null, CultureInfo culture = null)
        {
            if (converter == null)
                converter = TypeDescriptor.GetConverter(targetType);

            if (converter.CanConvertFrom(context, typeof(string)))
            {
                if (culture == null)
                    culture = CultureInfo.InvariantCulture;

                result = converter.ConvertFrom(context, culture, value);
                return true;
            }
            else
            {
                result = targetType.GetDefaultValue();
                return false;
            }
        }

        public static bool TryConvertTo<T>(this string value, out T result, TypeConverter converter = null, ITypeDescriptorContext context = null, CultureInfo culture = null)
        {
            object objectResult;
            var success = value.TryConvertTo(typeof(T), out objectResult, converter, context, culture);
            result = (T)objectResult;
            return success;
        }

        public static Stream ToStream(this string value)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(value);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static string ToUnderscoresNaming(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            value = value.Trim();
            StringBuilder builder = new StringBuilder();
            var firstChar = value[0];
            var previousUpper = char.IsUpper(firstChar);
            builder.Append(char.ToLower(firstChar));

            for(var i = 1; i < value.Length; ++i)
            {
                var isUpper = char.IsUpper(value[i]);
                var lowerChar = char.ToLower(value[i]);
                if (!isUpper || previousUpper)
                    builder.Append(lowerChar);
                else
                    builder.Append('_').Append(lowerChar);

                previousUpper = isUpper;
            }

            return builder.ToString();
        }

        public static string Replace(this string value, char[] oldChars, char newChar)
        {
            var replacedString = value;
            foreach (var chr in oldChars)
                replacedString = replacedString.Replace(chr, newChar);

            return replacedString;
        }
    }
}
