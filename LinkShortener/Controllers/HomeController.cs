using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LinkShortener.Models;
using LinkShortener.Services;
using LinkShortener.Services.Results.Validation;

namespace LinkShortener.Controllers;

public class HomeController : Controller
{
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ILinkValidator _linkValidator;
    private readonly ITokenValidator _tokenValidator;
    private readonly IHttpContextAccessor _contextAccessor;

    public HomeController(ITokenGenerator tokenGenerator, ILinkValidator validator, ITokenValidator tokenValidator, IHttpContextAccessor contextAccessor)
    {
        _tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
        _linkValidator = validator ?? throw new ArgumentNullException(nameof(validator));
        _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
        _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
    }

    [HttpGet]
    public async  Task<IActionResult> Index(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return View(nameof(HomeController.Index));
        }

        var tokenValidationResult = await _tokenValidator.Validate(token);
        
        if(tokenValidationResult is ValidTokenResult tokenResult)
        {
            return Redirect(tokenResult.Link.AbsoluteUri);
        }

        return NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(GenerateLinkViewModel model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        if (string.IsNullOrWhiteSpace(model.Link))
        {
            ModelState.AddModelError(string.Empty, "Please provide a link");
            return View(nameof(HomeController.Index));
        }

        var validationResult = _linkValidator.Validate(model.Link);

        if (validationResult is not ValidUriResult validUriResult)
        {
            var errorResult = validationResult as InvalidUriResult;
            ModelState.AddModelError(string.Empty, errorResult?.Message);
            return View(nameof(HomeController.Index));
        }
        
        var token = await _tokenGenerator.Generate(validUriResult.ValidUri);
        
        var httpContext = _contextAccessor.HttpContext;
        
        return RedirectToAction("Success", new SuccessViewModel()
        {
            Link = $"{httpContext?.Request.Scheme}://{httpContext?.Request.Host}/?token={token}"
        });
    }

    [HttpGet]
    public IActionResult Success(SuccessViewModel model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        return View("Success", model);
    }

    [HttpPost]
    public IActionResult Success() => RedirectToAction(nameof(HomeController.Index));
}