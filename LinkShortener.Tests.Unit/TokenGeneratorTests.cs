using FluentAssertions;
using LinkShortener.Configuration;
using LinkShortener.Exceptions;
using LinkShortener.Models;
using LinkShortener.Stores;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;

namespace LinkShortener.Tests.Unit;

public class TokenGeneratorTests
{
    private Mock<ISystemClock> _systemClock = new Mock<ISystemClock>();
    private Mock<IShortenedLinkStore> _store = new Mock<IShortenedLinkStore>();
    private Mock<IOptions<TokenOptions>> _options = new Mock<IOptions<TokenOptions>>();

    public TokenGeneratorTests()
    {
        var tokenOptions = new TokenOptions()
        {
            Size = 3,
            AllowedCharacters = "a",
            RetryCount = 3
        };

        _options?.Setup(x => x.Value)
            .Returns(tokenOptions);
    }
    private TokenGenerator CreateSut()
    {
        return new TokenGenerator(_systemClock?.Object, _store?.Object, _options?.Object);
    }
    
    [Fact]
    public void ctor_WhenStoreNull()
    {
        _store = null;

        Action comparison = () => CreateSut();

        comparison.Should().Throw<ArgumentNullException>();
    }
    
    [Fact]
    public void ctor_WhenSystemClockNull()
    {
        _systemClock = null;

        Action comparison = () => CreateSut();

        comparison.Should().Throw<ArgumentNullException>();
    }
    
    [Fact]
    public void ctor_WhenOptionsNull()
    {
        _options = null;

        Action comparison = () => CreateSut();

        comparison.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task Generate_WhenRetryCountReached_ShouldThrowTokenGenerationFailedException()
    {
        _store.Setup(x => x.Get(It.IsAny<string>()))
            .ReturnsAsync(new ShortenedLink(new Uri("https://test.fake"), "token", DateTimeOffset.UnixEpoch));
        
        Func<Task<string>> task = () => CreateSut().Generate(new Uri("https://test.fake"));

        await task.Should().ThrowAsync<TokenGenerationFailedException>();
        
        _store.Verify(x => x.Get(It.IsAny<string>()), Times.Exactly(3));
    }

    [Fact]
    public async Task Generate_WhenTokenNotFoundInStore_ExpectShortenedLinkGenerated()
    {
        var link = new ShortenedLink(new Uri("https://test.fake"), "token", DateTimeOffset.UnixEpoch);
        var sut = CreateSut();

        var result = await sut.Generate(link.Uri);

        result.Should().Be("aaa");
    }
    [Fact]
    public void GenerateTokenString_WhenCalled_ShouldGenerateToken()
    {
        var sut = CreateSut();

        var result = sut.GenerateTokenString();

        result.Should().NotBeNull();
        result.Should().Be("aaa");

    }
}