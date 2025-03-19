namespace BuildingBlocks.Authentication;

public class JWTSettings
{
    public string? Authority { get; set; }

    public string? Issuer { get; set; }

    public string? Audience { get; set; }

    public string? Key { get; set; }

    public double DurationInMinutes { get; set; }
}

