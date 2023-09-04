namespace Impulse.Models;

/// <summary>
/// Represents a chat room.
/// </summary>
[Immutable]
[GenerateSerializer]
public sealed record ChatRoom(
    [property: Id(1)] Guid Guid,
    [property: Id(2)] string Name,
    [property: Id(3)] DateTimeOffset Created = default,
    [property: Id(4)] DateTimeOffset Updated = default,
    [property: Id(5)] Guid ETag = default);