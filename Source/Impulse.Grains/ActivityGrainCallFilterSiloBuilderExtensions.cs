namespace Impulse.Grains;

public static class ActivityGrainCallFilterSiloBuilderExtensions
{
    public static ISiloBuilder AddActivityGrainCallFilter(this ISiloBuilder builder, Action<ActivityGrainCallFilterOptions> configure)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(configure);

        return builder
            .AddIncomingGrainCallFilter<ActivityGrainCallFilter>()
            .ConfigureServices(services =>
            {
                services
                    .AddOptions<ActivityGrainCallFilterOptions>()
                    .Configure(configure)
                    .ValidateDataAnnotations();
            });
    }
}