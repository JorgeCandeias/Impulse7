namespace Impulse.WebApi.Middleware;

internal class RequestContextMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        RequestContext.Set("TraceId", Activity.Current?.Id);

        await next.Invoke(context);
    }
}