using LinkShortener.Services.Results.Validation;

namespace LinkShortener.Services;

public interface ILinkValidator
{
    public UriValidationResult Validate(string uri);
}