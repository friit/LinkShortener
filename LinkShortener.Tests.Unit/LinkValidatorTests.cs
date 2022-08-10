using FluentAssertions;
using LinkShortener.Services.Results.Validation;
using LinkShortener.Validators;
using Microsoft.Extensions.Logging.Abstractions;

namespace LinkShortener.Tests.Unit;

public class LinkValidatorTests
{
    private Mock<IUriValidator> _uriValidator = new Mock<IUriValidator>();

    private LinkValidator CreateSut()
    {
        return new LinkValidator(new List<IUriValidator>() { _uriValidator.Object }, new NullLogger<ILinkValidator>());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]    
    public void Validate_WhenUriNullEmptyWhitespace_ExpectArgumentNullException(string uri)
    {
        var sut = CreateSut();

        Action action = () => CreateSut().Validate(uri);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Validate_WhenUriCreationFails_ExpectCreationFailedResultReturned()
    {
        var sut = CreateSut();

        var result = sut.Validate("uri");
        result.Should().BeOfType<CreationFailedResult>();
    }

    [Fact]
    public void When_UriCreatedAndValidatorFailed_ExpectInvalidUriResultReturned()
    {
        var uri = "https://test.fake/";

        _uriValidator.Setup(x => x.Validate(It.Is<Uri>(x => x.AbsoluteUri == uri)))
            .Returns(new InvalidSchemeResult(""));

        var sut = CreateSut();

        var result = sut.Validate(uri);
        result.Should().BeOfType<InvalidSchemeResult>();
    }
    
    [Fact]
    public void When_UriCreatedAndValidatorSucceed_ExpectInvalidUriResultReturned()
    {
        var uri = "https://test.fake/";

        _uriValidator.Setup(x => x.Validate(It.Is<Uri>(x => x.AbsoluteUri == uri)))
            .Returns(new ValidUriResult(new Uri(uri)));

        var sut = CreateSut();

        var result = sut.Validate(uri);
        var uriResult = result as ValidUriResult;
        uriResult?.ValidUri.AbsoluteUri.Should().Be(uri);
    }
}