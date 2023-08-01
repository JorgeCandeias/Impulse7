namespace Impulse.Models;

/// <summary>
/// Holds chat room statistics for the cluster.
/// </summary>
[Immutable]
[GenerateSerializer]
public record ActiveChatRoomClusterStats(
    [property: Id(1)] int SiloCount,
    [property: Id(2)] int RoomCount,
    [property: Id(3)] int UserCount,
    [property: Id(4)] int MessageCount);