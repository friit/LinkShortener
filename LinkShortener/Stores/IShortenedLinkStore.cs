using LinkShortener.Models;

namespace LinkShortener.Stores;

public interface IShortenedLinkStore
{
    Task Store(ShortenedLink token);
    Task<ShortenedLink?> Get(string token);
}