namespace LinkShortener.Exceptions;

public class TokenGenerationFailedException : Exception
{
    public TokenGenerationFailedException()
    {
        
    }

    public TokenGenerationFailedException(string message) : base(message)
    {
        
    }
    
    public TokenGenerationFailedException(string? message, Exception? innerException)
        : base(message, innerException)
    {
        
    }
}