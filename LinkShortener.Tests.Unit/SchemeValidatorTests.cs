using FluentAssertions;
using LinkShortener.Services.Results.Validation;
using LinkShortener.Validators;

namespace LinkShortener.Tests.Unit;

public class SchemeValidatorTests
{
    private SchemeValidator CreateSut()
    {
        return new SchemeValidator();
    }

    [Fact]
    public void Validate_WhenUriNull_ExpectArgumentNullException()
    {
        var sut = CreateSut();
        Action result = () => sut.Validate((Uri)null);
        result.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Validate_WhenUriDoesNotStartWithHttps_ExpectInvalidSchemeResultt()
    {
        var uri = "http://test.fake/";
        var sut = CreateSut();
        var result = sut.Validate(new Uri(uri));
        result.Should().BeOfType<InvalidSchemeResult>();
    }

    [Fact]
    public void Validate_WhenUriStartsWithHttps_ExpectValidateUriResult()
    {
        var uri = "https://test.fake/";
        var sut = CreateSut();
        var result = sut.Validate(new Uri(uri));
        var validResult = result as ValidUriResult;
        validResult.Should().NotBeNull();
        validResult.ValidUri.AbsoluteUri.Should().Be(uri);


    }
}