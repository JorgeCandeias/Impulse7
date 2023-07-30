namespace Impulse.Core.Extensions;

public static class ObjectTaskExtensions
{
    /// <summary>
    /// Returns the specified value wrapped in <see cref="Task.FromResult{TResult}(TResult)"/>.
    /// </summary>
    public static Task<T> AsTaskResult<T>(this T value)
    {
        return Task.FromResult(value);
    }
}