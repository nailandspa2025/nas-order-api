using System.Text.RegularExpressions;
using BuildingBlocks.Common.Helpers;

namespace BuildingBlocks.Common.Extensions;


public static class StringExtensions
{
    public static string SplitCamelCase(this string str)
    {
        return Regex.Replace(
            Regex.Replace(
                str,
                @"(\P{Ll})(\P{Ll}\p{Ll})",
                "$1 $2"
            ),
            @"(\p{Ll})(\P{Ll})",
            "$1 $2"
        );
    }

    
    public static bool In(this string me, params string[] set)
    {
        return set.Contains(me);
    }

    public static bool NotIn(this string me, params string[] set)
    {
        return !set.Contains(me);
    }

    public static bool NotIn(this string me, IEnumerable<string> set)
    {
        return !set.Contains(me);
    }

    
    public static bool IsNullOrEmpty(this string @this)
    {
        return string.IsNullOrEmpty(@this);
    }

    
    public static bool IsNotNullNorEmpty(this string @this)
    {
        return !string.IsNullOrEmpty(@this);
    }

    public static bool IsBase64(this string @this)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(@this);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public static string AsNormalize(this string @this)
    {
        if (string.IsNullOrWhiteSpace(@this))
        {
            return string.Empty;
        }

        return StringHelper.ToNonAccentVietnamese(@this.Trim()).ToUpper();
    }
}
