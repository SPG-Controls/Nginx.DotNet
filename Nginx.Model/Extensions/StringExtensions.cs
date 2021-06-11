using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Nginx.Model.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Get a list of all indexes of a given string match
        /// </summary>
        public static IEnumerable<int> IndexOfAll(this string input, string match)
        {
            var indexes = new List<int>();

            int index = -1;
            while (true)
            {
                index = input.IndexOf(match, index + 1);
                if (index < 0)
                    break;

                indexes.Add(index);
            }

            return indexes;
        }

        /// <summary>
        /// Strip a comment from a YAML line
        /// </summary>
        public static string StripLineComment(this string input)
        {
            int commentIndex = input.IndexOf('#');

            return commentIndex > 0 ? input.Remove(commentIndex) : input;
        }

        public static int RegexIndexOf(this string str, string pattern, int startIndex)
        {
            Regex rx = new Regex(pattern, RegexOptions.Multiline);
            var m = rx.Match(str, startIndex);
            return m.Success ? m.Index : -1;
        }
    }
}
