namespace BuildingBlocks.Common.API.Error;

public class Resources
{

    private static global::System.Resources.ResourceManager resourceMan;

    private static global::System.Globalization.CultureInfo resourceCulture;

    [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal Resources()
    {
    }

    /// <summary>
    ///   Returns the cached ResourceManager instance used by this class.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
    public static global::System.Resources.ResourceManager ResourceManager
    {
        get
        {
            if (object.ReferenceEquals(resourceMan, null))
            {
                global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DHubPlatform.BuildingBlocks.Common.API.Error.Resources", typeof(Resources).Assembly);
                resourceMan = temp;
            }
            return resourceMan;
        }
    }

    /// <summary>
    ///   Overrides the current thread's CurrentUICulture property for all
    ///   resource lookups using this strongly typed resource class.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
    public static global::System.Globalization.CultureInfo Culture
    {
        get
        {
            return resourceCulture;
        }
        set
        {
            resourceCulture = value;
        }
    }

    /// <summary>
    ///   Looks up a localized string similar to Có sự cố không mong muốn xảy ra. Xin vui lòng liên hệ bộ phận chăm sóc khách hàng..
    /// </summary>
    public static string DEFAULT_ERROR
    {
        get
        {
            return ResourceManager.GetString("DEFAULT_ERROR", resourceCulture);
        }
    }

    /// <summary>
    ///   Looks up a localized string similar to Mã OTP đã hết hạn..
    /// </summary>
    public static string OTP_EXPIRED
    {
        get
        {
            return ResourceManager.GetString("OTP_EXPIRED", resourceCulture);
        }
    }

    /// <summary>
    ///   Looks up a localized string similar to Mã OTP không hợp lệ..
    /// </summary>
    public static string OTP_INVALID
    {
        get
        {
            return ResourceManager.GetString("OTP_INVALID", resourceCulture);
        }
    }

    /// <summary>
    ///   Looks up a localized string similar to Thông tin người dùng &apos;{0}&apos; không tồn tại..
    /// </summary>
    public static string USER_NOTFOUND
    {
        get
        {
            return ResourceManager.GetString("USER_NOTFOUND", resourceCulture);
        }
    }
}

