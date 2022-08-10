using LinkShortener.Services.Results.Validation;

namespace LinkShortener.Services;

public interface ITokenValidator
{
    Task<TokenValidationResult> Validate(string token);
}