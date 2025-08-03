using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace intapscamis.camis.domain.Report
{
    
    public static class StringExtension
    {
        public static string Beautify(this string str, int StartIndex)
        {
            var n = str.Substring(StartIndex).Split('-');
            List<string> s = new List<string>();
            foreach (var word in n)
            {
                var newWord = Char.ToUpperInvariant(word[0]) + word.Substring(1);
                s.Add(newWord);
            }

            return String.Join(' ', s);

        }

        public static string GetUpin(this string str)
        {
            var split = str.Split('-').LastOrDefault() ;
            return split;
        }

        public static string GetTag(this string tag)
        {
            var sp = tag.Split('-');
            sp = sp.Take(sp.Count() - 1).ToArray();
            return String.Join('-', sp);
        }

        public static string Woreda(this string str)
        {
            var s = str.Split('/').Take(3);
            return String.Join('/', s);
        }

        public static string Zone(this string str)
        {
            var s = str.Split('/').Take(2);
            return String.Join('/', s);
        }

        public static string Region(this string str)
        {
            var s = str.Split('/').Take(1);
            return String.Join('/', s);
        }

        public static string Beautify(this string str)
        {
            var n = str.Split('-');
            List<string> s = new List<string>();
            foreach (var word in n)
            {
                var newWord = Char.ToUpperInvariant(word[0]) + word.Substring(1);
                s.Add(newWord);
            }

            return String.Join(' ', s);

        }
    }
}
