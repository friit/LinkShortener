using LinkShortener.Services;
using LinkShortener.Services.Results.Validation;

namespace LinkShortener.Validators;


public class SchemeValidator : IUriValidator
{
    public UriValidationResult Validate(Uri uri)
    {
        if (uri == null) throw new ArgumentNullException(nameof(uri));
        
        if (!uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
        {
            return new InvalidSchemeResult($"URI Scheme validation failed with Uri of {uri}");
        }

        return new ValidUriResult(uri);
    }
}

