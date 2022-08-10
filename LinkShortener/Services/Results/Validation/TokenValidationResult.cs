using LinkShortener.Models;

namespace LinkShortener.Services.Results.Validation;

public abstract class TokenValidationResult
{
    public abstract bool Succeeded { get;}
}
public class InvalidTokenResult : TokenValidationResult
{
    public override bool Succeeded => false;
    
    public InvalidTokenResult()
    {
        
    }
}

public class ValidTokenResult : TokenValidationResult
{
    public ShortenedLink ShortenedLink { get; set; }
    public Uri Link => ShortenedLink.Uri;
    public override bool Succeeded => true;

    public ValidTokenResult(ShortenedLink shortenedLink)
    { 
        ShortenedLink = shortenedLink;
    }
}