namespace Impulse.Models;

/// <summary>
/// Holds statistics for a chat room.
/// </summary>
[Immutable]
[GenerateSerializer]
public sealed record ActiveChatRoomStats(
    [property: Id(1)] string Name,
    [property: Id(2)] int UserCount,
    [property: Id(3)] int MessageCount);