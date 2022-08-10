namespace LinkShortener.Services.Results.Validation;

public abstract class InvalidUriResult : UriValidationResult 
{ 
    public string Error { get; init; }
    public string Message { get; init; }


    public InvalidUriResult(string error, string message)
    {
        Error = error;
        Message = message;
    }
}