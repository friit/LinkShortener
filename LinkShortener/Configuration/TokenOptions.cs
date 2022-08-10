namespace LinkShortener.Configuration;

public class TokenOptions
{
    public string AllowedCharacters { get; set; } = "1234567890abcdefghijklmnopqrstuvwxyz";
    public int Size { get; set; } = 6;
    public int RetryCount { get; set; } = 3;
}