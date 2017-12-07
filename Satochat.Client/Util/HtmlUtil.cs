using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Satochat.Client.Util {
    public static class HtmlUtil {
        // https://weblog.west-wind.com/posts/2009/Feb/05/Html-and-Uri-String-Encoding-without-SystemWeb
        public static string HtmlEncode(string text) {
            if (text == null)
                return null;

            StringBuilder sb = new StringBuilder(text.Length);

            int len = text.Length;
            for (int i = 0; i < len; i++) {
                switch (text[i]) {

                    case '<':
                        sb.Append("&lt;");
                        break;
                    case '>':
                        sb.Append("&gt;");
                        break;
                    case '"':
                        sb.Append("&quot;");
                        break;
                    case '&':
                        sb.Append("&amp;");
                        break;
                    default:
                        if (text[i] > 159) {
                            // decimal numeric entity
                            sb.Append("&#");
                            sb.Append(((int)text[i]).ToString(CultureInfo.InvariantCulture));
                            sb.Append(";");
                        } else
                            sb.Append(text[i]);
                        break;
                }
            }
            return sb.ToString();
        }

        public static string HtmlDecode(string text) {
            string pattern = @"&(lt|gt|quot|amp|#\d+);";
            return Regex.Replace(text, pattern, m => {
                string entity = m.Groups[1].Value;
                switch (entity) {
                    case "lt":
                        return "<";
                    case "gt":
                        return ">";
                    case "quot":
                        return "\"";
                    case "amp":
                        return "&";
                    default:
                        return m.Groups[1].Value;
                }
            });
        }
    }
}
