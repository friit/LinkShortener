using LinkShortener.Models;

namespace LinkShortener.Stores;

public class InMemoryShortenedLinkStore : IShortenedLinkStore
{
    private readonly List<ShortenedLink> _store = new List<ShortenedLink>();
    public Task Store(ShortenedLink token)
    {
        _store.Add(token);

        return Task.CompletedTask;
    }

    public Task<ShortenedLink?> Get(string token)
    {
        return Task.FromResult(_store.SingleOrDefault(x => x.Token == token));
    }
}