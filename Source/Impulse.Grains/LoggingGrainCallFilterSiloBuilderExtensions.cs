namespace Impulse.Grains;

public static class LoggingGrainCallFilterSiloBuilderExtensions
{
    public static ISiloBuilder AddLoggingGrainCallFilter(this ISiloBuilder builder, Action<LoggingGrainCallFilterOptions> configure)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(configure);

        return builder
            .AddIncomingGrainCallFilter<LoggingGrainCallFilter>()
            .ConfigureServices(services =>
            {
                services
                    .AddOptions<LoggingGrainCallFilterOptions>()
                    .Configure(configure)
                    .ValidateDataAnnotations();
            });
    }
}