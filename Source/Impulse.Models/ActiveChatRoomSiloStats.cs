namespace Impulse.Models;

/// <summary>
/// Holds chat room statistics for a silo.
/// </summary>
[Immutable]
[GenerateSerializer]
public sealed record ActiveChatRoomSiloStats(
    [property: Id(1)] SiloAddress SiloAddress,
    [property: Id(2)] int RoomCount,
    [property: Id(3)] int UserCount,
    [property: Id(4)] int MessageCount);