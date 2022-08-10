using FluentAssertions;
using LinkShortener.Models;
using LinkShortener.Services.Results.Validation;
using LinkShortener.Stores;

namespace LinkShortener.Tests.Unit;

public class TokenValidatorTests
{
    private Mock<IShortenedLinkStore> _store = new Mock<IShortenedLinkStore>();

    private TokenValidator CreateSut()
    {
        return new TokenValidator(_store?.Object);
    }
    

    [Fact]
    public void ctor_WhenStoreNull()
    {
        _store = null;

        Action comparison = () => CreateSut();

        comparison.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Validate_WhenTokenNullEmptyWhitespace_ExpectArgumentNullException(string token)
    {
        Func<Task<TokenValidationResult>> task = () => CreateSut().Validate(token);
        await task.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Validate_WhenStoreReturnsNull_ExpectInvalidTokenResult()
    {
        var token = "token";

        _store.Setup(x => x.Get(token))
            .ReturnsAsync((ShortenedLink)null);

        var sut = CreateSut();

        var result = await sut.Validate(token);

        result.Should().BeOfType<InvalidTokenResult>();
    }

    [Fact]
    public async Task Validate_WhenStoreReturnsShortenedLink_ExpectValidTokenResult()
    {
        var token = "token";

        _store.Setup(x => x.Get(token))
            .ReturnsAsync(new ShortenedLink(new Uri("https://test.fake"), token, DateTimeOffset.UnixEpoch));

        var sut = CreateSut();

        var result = await sut.Validate(token);

        result.Should().BeOfType<ValidTokenResult>();
    }
}