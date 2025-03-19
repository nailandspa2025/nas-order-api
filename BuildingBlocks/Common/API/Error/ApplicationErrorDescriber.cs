namespace BuildingBlocks.Common.API.Error;

public class ApplicationErrorDescriber
{
    public virtual ApplicationError DefaultError()
    {
        return new ApplicationError
        {
            Code = nameof(DefaultError),
            Description = Resources.DEFAULT_ERROR
        };
    }

    public virtual ApplicationError OtpExpired()
    {
        return new ApplicationError
        {
            Code = nameof(OtpExpired),
            Description = Resources.OTP_EXPIRED
        };
    }

    public virtual ApplicationError OtpInvalid()
    {
        return new ApplicationError
        {
            Code = nameof(OtpInvalid),
            Description = Resources.OTP_INVALID
        };
    }

    public virtual ApplicationError UserNotFound(string userIdentifier)
    {
        return new ApplicationError
        {
            Code = nameof(UserNotFound),
            Description = string.Format(Resources.USER_NOTFOUND, userIdentifier)
        };
    }
}

