using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;
using LinkShortener.Configuration;
using LinkShortener.Exceptions;
using LinkShortener.Models;
using LinkShortener.Stores;

namespace LinkShortener.Services;

public class TokenGenerator : ITokenGenerator
{
    private readonly ISystemClock _systemClock;
    private readonly IShortenedLinkStore _store;
    private readonly TokenOptions _options;

    public TokenGenerator(ISystemClock systemClock, IShortenedLinkStore store, IOptions<TokenOptions> options)
    {
        _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }
    public async Task<string> Generate(Uri link)
    {
        var count = 0;

        while (count < _options.RetryCount)
        {
            var token = GenerateTokenString();
            if (await _store.Get(token) != null)
            {
                count += 1;
                continue;
            }

            var shortenLink = new ShortenedLink(link, token, _systemClock.UtcNow);

            await _store.Store(shortenLink);

            return shortenLink.Token;
        }

        throw new TokenGenerationFailedException();
    }

    public string GenerateTokenString()
    {
        var rnd = new Random();
        
        var chars = Enumerable.Range(0, _options.Size)
            .Select(x => $"{_options.AllowedCharacters[rnd.Next(0, _options.AllowedCharacters.Length)]}");

        return string.Concat(chars);
    }
}