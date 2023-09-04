using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Impulse.Models.Orleans;

public static class ImpulseClientConnectionRetryClientBuilderExtensions
{
    public static IClientBuilder UseImpulseClientConnectionRetryFilter(this IClientBuilder builder, Action<ImpulseClientConnectionRetryOptions> configure)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(configure);

        return builder
            .UseConnectionRetryFilter<ImpulseClientConnectionRetryFilter>()
            .ConfigureServices(services =>
            {
                services
                    .AddOptions<ImpulseClientConnectionRetryOptions>()
                    .Configure(configure)
                    .ValidateDataAnnotations();
            });
    }
}