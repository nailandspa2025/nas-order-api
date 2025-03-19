using System.Text.RegularExpressions;

namespace BuildingBlocks.Common.Helpers;

public static class StringHelper
{
    const string PhoneNumberRegex = @"^(0|\+84)(\d{3})(\d{3})(\d{3})$";

    private static readonly string[] VietNamChar = new string[]
   {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
   };

    public static bool ValidatePhoneNumberFormat(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return false;
        }

        return Regex.IsMatch(str, PhoneNumberRegex, RegexOptions.IgnoreCase);
    }

    public static string ToNonAccentVietnamese(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        //replace and mark each character    
        for (int i = 1; i < VietNamChar.Length; i++)
        {
            for (int j = 0; j < VietNamChar[i].Length; j++)
                str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
        }
        return str;
    }
}

