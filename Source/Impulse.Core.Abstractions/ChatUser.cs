namespace Impulse.Core;

/// <summary>
/// Represents a chat user.
/// </summary>
/// <param name="Id">The unique identifier of the chat user.</param>
/// <param name="Name">The name of the chat user.</param>
/// <param name="Created">The time at which the chat user was created.</param>
public record ChatUser(
    [property: Id(1)] string Name,
    [property: Id(2)] DateTimeOffset Created);