namespace Impulse.Core;

/// <summary>
/// Represents a chat room.
/// </summary>
/// <param name="Id">The unique identifier of the chat room.</param>
/// <param name="Name">The name of the chat room.</param>
/// <param name="Created">The time at which the chat room was created.</param>
public record ChatRoom(
    [property: Id(1)] string Name,
    [property: Id(2)] DateTimeOffset Created);
