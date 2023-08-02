namespace Impulse.Grains;

internal partial class LoggingGrainCallFilter : IIncomingGrainCallFilter
{
    public LoggingGrainCallFilter(IOptions<LoggingGrainCallFilterOptions> options, ILogger<LoggingGrainCallFilter> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    private readonly LoggingGrainCallFilterOptions _options;
    private readonly ILogger _logger;

    public Task Invoke(IIncomingGrainCallContext context)
    {
        // passthrough calls to grains not belonging to the allowed assemblies
        if (!_options.AllowedAssemblies.Contains(context.Grain.GetType().Assembly))
        {
            return context.Invoke();
        }

        // passthrough calls without an activity id
        if (RequestContext.Get("ActivityId") is not string activityId)
        {
            return context.Invoke();
        }

        // otherwise handle the call
        return InvokeCore(activityId, context);
    }

    private async Task InvokeCore(string activityId, IIncomingGrainCallContext context)
    {
        // derive a name for the activity
        var identity = (context.Grain as Grain)?.IdentityString;
        var method = context.ImplementationMethod.Name;
        var name = $"{identity}/{method}";

        // start measuring the current call
        var started = DateTimeOffset.UtcNow;
        var watch = Stopwatch.StartNew();

        try
        {
            await context.Invoke();

            LogOK(name, activityId, started, DateTimeOffset.UtcNow, watch.Elapsed);
        }
        catch (Exception ex)
        {
            LogFail(ex, activityId, name, started, DateTimeOffset.UtcNow, watch.Elapsed);
        }
    }

    [LoggerMessage(1, LogLevel.Information, "Method {Method} with activity {ActivityId} started at {Started} and completed at {Completed} taking {Elapsed}")]
    private partial void LogOK(string method, object activityId, DateTimeOffset started, DateTimeOffset completed, TimeSpan elapsed);

    [LoggerMessage(2, LogLevel.Error, "Method {Method} with trace {ActivityId} started at {Started} and completed at {Completed} taking {Elapsed}")]
    private partial void LogFail(Exception ex, object method, string activityId, DateTimeOffset started, DateTimeOffset completed, TimeSpan elapsed);
}