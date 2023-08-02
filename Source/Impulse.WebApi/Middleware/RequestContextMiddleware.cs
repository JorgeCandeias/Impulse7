namespace Impulse.WebApi.Middleware;

internal class RequestContextMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        RequestContext.Set("TraceId", context.TraceIdentifier);
        RequestContext.Set("ActivityId", Activity.Current?.Id);

        await next.Invoke(context);
    }
}