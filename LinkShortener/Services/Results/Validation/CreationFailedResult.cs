namespace LinkShortener.Services.Results.Validation;

public class CreationFailedResult : InvalidUriResult
{        
    const string ERROR = "Invalid URI";
    public CreationFailedResult() : base(ERROR, "")
    {
    }
}