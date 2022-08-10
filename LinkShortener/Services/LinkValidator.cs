using LinkShortener.Services.Results.Validation;
using LinkShortener.Validators;

namespace LinkShortener.Services;

public class LinkValidator : ILinkValidator
{
    private readonly IEnumerable<IUriValidator> _uriValidators;
    private readonly ILogger<ILinkValidator> _logger;

    public LinkValidator(IEnumerable<IUriValidator> uriValidators, ILogger<ILinkValidator> logger)
    {
        _uriValidators = uriValidators ?? throw new ArgumentNullException(nameof(uriValidators));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public UriValidationResult Validate(string uri)
    {
        if (string.IsNullOrWhiteSpace(uri)) throw new ArgumentNullException(nameof(uri));
        
        if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri? validUri))
        {
            _logger.LogError($"URI Create failed with Uri of: {uri}");
            return new CreationFailedResult();
        }

        foreach (var validator in _uriValidators)
        {
            var result = validator.Validate(validUri);

            if (result is InvalidUriResult invalidUriResult) 
            {
                _logger.LogError($"ERROR {invalidUriResult.Error}: {invalidUriResult.Message}");
                return result;
            }
        }

        return new ValidUriResult(validUri);
    }
}