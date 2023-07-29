using Microsoft.Extensions.Options;

namespace Impulse.Grains;

internal class ActivityGrainCallFilter : IIncomingGrainCallFilter
{
    public ActivityGrainCallFilter(IOptions<ActivityGrainCallFilterOptions> options, ActivitySource source)
    {
        _source = source;
        _options = options.Value;
    }

    private readonly ActivitySource _source;
    private readonly ActivityGrainCallFilterOptions _options;

    public Task Invoke(IIncomingGrainCallContext context)
    {
        // passthrough calls to grains not belonging to the allowed assemblies
        if (!_options.AllowedAssemblies.Contains(context.Grain.GetType().Assembly))
        {
            return context.Invoke();
        }

        // otherwise handle the call
        return InvokeCore(context);
    }

    private async Task InvokeCore(IIncomingGrainCallContext context)
    {
        // derive a name for the activity
        var identity = (context.Grain as Grain)?.IdentityString;
        var method = context.ImplementationMethod.Name;
        var name = $"{identity}/{method}";

        // create the activity for the current call
        using var activity = _source.StartActivity(name, ActivityKind.Internal);
        activity?.SetTag("TraceId", RequestContext.Get("TraceId"));

        // measure the target under the activity
        await context.Invoke();
    }
}