namespace LinkShortener.Services;

public interface ITokenGenerator
{
    public Task<string> Generate(Uri link);
}