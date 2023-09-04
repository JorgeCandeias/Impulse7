using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impulse.Models.Orleans;

internal partial class ImpulseClientConnectionRetryFilter : IClientConnectionRetryFilter
{
    public ImpulseClientConnectionRetryFilter(ILogger<ImpulseClientConnectionRetryFilter> logger, IOptions<ImpulseClientConnectionRetryOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    private readonly ILogger _logger;
    private readonly ImpulseClientConnectionRetryOptions _options;

    public async Task<bool> ShouldRetryConnectionAttempt(Exception exception, CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            return false;
        }

        LogError(exception, _options.Period);

        await Task.Delay(_options.Period, cancellationToken);

        return true;
    }

    #region Logging

    [LoggerMessage(1, LogLevel.Error, "Failed to connect to Orleans cluster and will retry in {Period}...")]
    private partial void LogError(Exception ex, TimeSpan period);

    #endregion Logging
}