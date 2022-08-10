using FluentAssertions;
using LinkShortener.Controllers;
using LinkShortener.Models;
using LinkShortener.Services.Results.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace LinkShortener.Tests.Unit;

public class HomeControllerTests
{
    private Mock<ITokenGenerator> _tokenGenerator = new Mock<ITokenGenerator>();
    private Mock<ILinkValidator> _linkValidator = new Mock<ILinkValidator>();
    private Mock<ITokenValidator> _tokenValidator = new Mock<ITokenValidator>();
    private Mock<IHttpContextAccessor> _contextAccessor = new Mock<IHttpContextAccessor>();
    
    private HomeController CreateSut()
    {
        return new HomeController(_tokenGenerator?.Object, _linkValidator?.Object, _tokenValidator?.Object, _contextAccessor?.Object);
    }
    
    [Fact]
    public void ctor_WhenTokenGeneratorNull()
    {
        _tokenGenerator = null;

        Action comparison = () => CreateSut();

        comparison.Should().Throw<ArgumentNullException>();
    }
    
    [Fact]
    public void ctor_WhenLinkValidatorNull()
    {
        _linkValidator = null;

        Action comparison = () => CreateSut();

        comparison.Should().Throw<ArgumentNullException>();
    }
    
    [Fact]
    public void ctor_WhenTokenValidatorNull()
    {
        _tokenValidator = null;

        Action comparison = () => CreateSut();

        comparison.Should().Throw<ArgumentNullException>();
    }
    
    [Fact]
    public void ctor_WhenContextAccessorNull()
    {
        _contextAccessor = null;

        Action comparison = () => CreateSut();

        comparison.Should().Throw<ArgumentNullException>();
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task IndexGet_WhenTokenIsNullEmptyWhiteSpace_ExpectViewReturned(string link)
    {
        var sut = CreateSut();

        var result = await sut.Index(link);

        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        
        viewResult?.ViewName.Should().Be(nameof(HomeController.Index));
        viewResult?.ViewData.ModelState.ErrorCount.Should().Be(0);
    }

    [Fact]
    public async Task IndexGet_WhenTokenValidatorFailed_ExpectNotFoundReturned()
    {
        var link = "link";

        _tokenValidator.Setup(x => x.Validate(link))
            .ReturnsAsync(new InvalidTokenResult());
        
        var sut = CreateSut();
        
        var result = await sut.Index(link);
        var statusCodeResult = result as StatusCodeResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult?.StatusCode.Should().Be(404);
    }
    
    [Fact]
    public async Task IndexGet_WhenTokenValidatorSuccess_ExpectRedirectResultReturned()
    {
        var link = new ShortenedLink(new Uri($"http://www.test.fake"), "token", DateTimeOffset.UnixEpoch);

        _tokenValidator.Setup(x => x.Validate(link.Token))
            .ReturnsAsync(new ValidTokenResult(link));
        
        var sut = CreateSut();
        
        var result = await sut.Index(link.Token);
        var redirectResult = result as RedirectResult;
        redirectResult.Should().NotBeNull();
        redirectResult?.Url.Should().Be(link.Uri.AbsoluteUri);
    }

    [Fact]
    public async Task IndexPost_WhenModelNull_ExpectArgumentNullException()
    {
        var sut = CreateSut();

        Func<Task<IActionResult>> task = () => sut.Index((GenerateLinkViewModel)null);
        await task.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]    
    public async Task IndexPost_WhenLinkNullEmptyWhitespace_ExpectViewReturnedWithErrors(string link)
    {
        var sut = CreateSut();

        var result = await sut.Index(new GenerateLinkViewModel(){Link = link});

        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        
        viewResult?.ViewName.Should().Be(nameof(HomeController.Index));
        viewResult?.ViewData.ModelState.ErrorCount.Should().Be(1);
    }

    [Fact]
    public async Task IndexPost_WhenLinkValidatorFailed_ExpectViewReturnedWithErrors()
    {
        var link = new ShortenedLink(new Uri($"http://www.test.fake"), "token", DateTimeOffset.UnixEpoch);

        _linkValidator.Setup(x => x.Validate(link.Uri.AbsoluteUri))
            .Returns(new InvalidSchemeResult(""));
        
        var sut = CreateSut();
        
        var result = await sut.Index(new GenerateLinkViewModel(){Link = link.Uri.AbsoluteUri});
        
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        
        viewResult?.ViewName.Should().Be(nameof(HomeController.Index));
        viewResult?.ViewData.ModelState.ErrorCount.Should().Be(1);
    }

    [Fact]
    public async Task IndexPost_WhenTokenGenerated_ExpectSuccessViewReturned()
    {        
        var link = new ShortenedLink(new Uri($"http://www.test.fake"), "token", DateTimeOffset.UnixEpoch);

        var sut = CreateSut();


        _linkValidator.Setup(x => x.Validate(link.Uri.AbsoluteUri))
            .Returns(new ValidUriResult(link.Uri));

        _tokenGenerator.Setup(x => x.Generate(link.Uri))
            .ReturnsAsync(link.Token);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "scheme";
        httpContext.Request.Host = new HostString("host");

        _contextAccessor.Setup(x => x.HttpContext)
            .Returns(httpContext);
        
        var result = await sut.Index(new GenerateLinkViewModel() { Link = link.Uri.AbsoluteUri });

        var redirectToAction = result as RedirectToActionResult;
        redirectToAction.Should().NotBeNull();
        redirectToAction?.ActionName.Should().Be(nameof(HomeController.Success));
        redirectToAction?.RouteValues.Should().ContainValue($"scheme://host/?token={link.Token}");
    }

    [Fact]
    public async Task Success_WhenModelNull_ExpectArgumentNullException()
    {
        var sut = CreateSut();

        Func<IActionResult> task = () =>  sut.Success((SuccessViewModel)null);

        task.Should().Throw<ArgumentNullException>();

    }

    [Fact]
    public async Task Success_WhenModelNotNull_ExpectViewReturned()
    {
        var sut = CreateSut();

        var result = sut.Success(new SuccessViewModel());
        
        var viewResult = result as ViewResult;
        viewResult?.Should().NotBeNull();
        viewResult?.ViewName.Should().Be(nameof(HomeController.Success));
    }

    [Fact]
    public async Task SuccessPost_ExpectRedirectToActionReturned()
    {
        var sut = CreateSut();
        
        var result = sut.Success();
        
        var redirectToAction = result as RedirectToActionResult;
        redirectToAction.Should().NotBeNull();
        redirectToAction?.ActionName.Should().Be(nameof(HomeController.Index));
    }
}