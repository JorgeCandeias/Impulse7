using System.Runtime.Serialization;

namespace Impulse.Core.Exceptions;

[Serializable]
[GenerateSerializer]
public class ConflictException : ApplicationException
{
    public ConflictException()
    {
    }

    public ConflictException(string? message) : base(message)
    {
    }

    public ConflictException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public ConflictException(Guid? currentETag, Guid? storedETag) : base($"Current ETag '{currentETag}' does not match stored ETag '{storedETag}'")
    {
        CurrentETag = currentETag;
        StoredETag = storedETag;
    }

    protected ConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        CurrentETag = (Guid?)info.GetValue(nameof(CurrentETag), typeof(Guid?))!;
        StoredETag = (Guid?)info.GetValue(nameof(StoredETag), typeof(Guid?))!;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);

        info.AddValue(nameof(CurrentETag), CurrentETag, typeof(Guid?));
        info.AddValue(nameof(StoredETag), StoredETag, typeof(Guid?));
    }

    [Id(1)]
    public Guid? CurrentETag { get; }

    [Id(2)]
    public Guid? StoredETag { get; }
}