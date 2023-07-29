namespace Impulse.Core;

/// <summary>
/// Holds chat room statistics for the cluster.
/// </summary>
[Immutable]
[GenerateSerializer]
public record ChatRoomClusterStats(
    [property: Id(1)] int SiloCount,
    [property: Id(2)] int RoomCount,
    [property: Id(3)] int UserCount,
    [property: Id(4)] int MessageCount);