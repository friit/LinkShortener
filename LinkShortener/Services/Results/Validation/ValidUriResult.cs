namespace LinkShortener.Services.Results.Validation;

public class ValidUriResult : UriValidationResult
{
    public Uri ValidUri { get; init; }
    
    public ValidUriResult(Uri validUri) 
    {
        ValidUri = validUri;
        Succeeded = true;
    }
}