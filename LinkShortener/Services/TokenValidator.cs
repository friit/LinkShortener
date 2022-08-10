using LinkShortener.Services.Results.Validation;
using LinkShortener.Stores;

namespace LinkShortener.Services;

public class TokenValidator : ITokenValidator
{
    private readonly IShortenedLinkStore _store;

    public TokenValidator(IShortenedLinkStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }
    public async Task<TokenValidationResult> Validate(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));

        var result = await _store.Get(token);

        if (result == null)
        {
            return new InvalidTokenResult();
        }

        return new ValidTokenResult(result);
    }
}