namespace Impulse.Models.Extensions;

public static class ObjectValueTaskExtensions
{
    /// <summary>
    /// Returns the specified value wrapped in <see cref="ValueTask.FromResult{TResult}(TResult)"/>.
    /// </summary>
    public static ValueTask<T> AsValueTaskResult<T>(this T value)
    {
        return ValueTask.FromResult(value);
    }
}