namespace LinkShortener.Services.Results.Validation;

public class InvalidSchemeResult : InvalidUriResult
{   
    const string ERROR = "Invalid scheme";

    public InvalidSchemeResult(string errorMessage) : base(ERROR, errorMessage)
    {
    }
}