using LinkShortener.Services.Results.Validation;

namespace LinkShortener.Validators;

public interface IUriValidator
{
    public UriValidationResult Validate(Uri uri);
}