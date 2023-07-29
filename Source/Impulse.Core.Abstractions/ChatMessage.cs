namespace Impulse.Core;

/// <summary>
/// Represents an individual chat message.
/// </summary>
[Immutable]
[GenerateSerializer]
public record ChatMessage(
    [property: Id(1)] Guid Guid,
    [property: Id(2)] string Room,
    [property: Id(3)] string User,
    [property: Id(4)] string Text,
    [property: Id(5)] DateTimeOffset Created = default,
    [property: Id(6)] DateTimeOffset Updated = default,
    [property: Id(7)] Guid ETag = default);