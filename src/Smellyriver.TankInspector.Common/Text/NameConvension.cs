using System.Text;

namespace Smellyriver.TankInspector.Common.Text
{
    public static class NameConvension
    {

        public static string CamelToCStyle(string name, bool keepNonAlphanumerics = false)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            var builder = new StringBuilder();

            builder.Append(char.ToLower(name[0]));

            var suppressUnderscore = char.IsUpper(name[0]);
            for (var i = 1; i < name.Length; ++i)
            {
                var chr = name[i];

                if(!char.IsLetterOrDigit(chr))
                {
                    if (keepNonAlphanumerics)
                    {
                        builder.Append(chr);
                        suppressUnderscore = true;
                        continue;
                    }
                    else
                    {
                        builder.Append('_');
                        suppressUnderscore = true;
                        continue;
                    }
                }

                if (char.IsUpper(chr))
                {
                    if (!suppressUnderscore)
                        builder.Append('_');

                    builder.Append(char.ToLower(chr));
                    suppressUnderscore = true;
                }
                else
                {
                    builder.Append(chr);
                    suppressUnderscore = false;
                }
            }

            return builder.ToString();
        }
    }
}
