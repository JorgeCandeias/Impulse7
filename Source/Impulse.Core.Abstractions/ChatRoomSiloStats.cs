using Orleans.Runtime;

namespace Impulse.Core;

/// <summary>
/// Holds chat room statistics for a silo.
/// </summary>
[Immutable]
[GenerateSerializer]
public record ChatRoomSiloStats(
    [property: Id(1)] SiloAddress Silo,
    [property: Id(2)] int RoomCount,
    [property: Id(3)] int UserCount,
    [property: Id(4)] int MessageCount);