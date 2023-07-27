namespace Impulse.Core;

/// <summary>
/// Represents an individual chat message.
/// </summary>
/// <param name="Id">The unique identifier of the chat message.</param>
/// <param name="Room">The room that this chat message belongs to.</param>
/// <param name="User">The user that created this chat message.</param>
/// <param name="Text">The content of this chat message.</param>
/// <param name="Created">The time at which this chat message was created.</param>
[Immutable]
[GenerateSerializer]
public record ChatMessage(
    [property: Id(1)] Guid Guid,
    [property: Id(2)] string Room,
    [property: Id(3)] string User,
    [property: Id(4)] string Text,
    [property: Id(5)] DateTimeOffset Created);