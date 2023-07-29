namespace Impulse.Core;

/// <summary>
/// Represents a chat room.
/// </summary>
public record ChatRoom(
    [property: Id(1)] Guid Guid,
    [property: Id(2)] string Name,
    [property: Id(3)] DateTimeOffset Created,
    [property: Id(4)] DateTimeOffset Updated,
    [property: Id(5)] Guid ETag);