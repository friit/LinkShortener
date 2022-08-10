namespace LinkShortener.Models;

public class ShortenedLink
{
    public Uri Uri { get; }
    public string Token { get; }
    public DateTimeOffset Created { get; }

    public ShortenedLink(Uri uri, string token, DateTimeOffset created)
    {
        Uri = uri ?? throw new ArgumentNullException(nameof(uri));
        Token = token ?? throw new ArgumentNullException(nameof(token));
        Created = created;
    }
}