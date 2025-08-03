using System;

namespace intapscamis.camis.domain.Extensions
{
    public static class GuidExtensions
    {
        public static Guid ToGuid(this string s)
        {
            return Guid.Parse(s);
        }
        
    }
    public static class StringExtensions
    {
        public static String addDelimitedListItem(String cur, String sep, String val)
        {
            if (String.IsNullOrEmpty(cur))
                return val;
            return cur + sep + val;
        }

    }
}